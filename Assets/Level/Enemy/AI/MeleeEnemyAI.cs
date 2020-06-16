using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    /// <summary>
    /// Jak daleko musze byt hrac, aby dostal damage
    /// </summary>
    public float attackRadius = 1.5f;

    public float playerDistance = 2f;

    /// <summary>
    /// Jak dlouho trvá, než se aplikuje damage
    /// </summary>
    public float attackDuration = 0.7f;

    public float waitTime = 0.8f;

    public float knockback = 1f;

    public override void Start()
    {
        base.Start();
        GoToPlayer(attackRadius);
    }

    protected override void WalkFinished()
    {
        Decide(false);
    }

    protected virtual IEnumerator attackSeq()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position,attackRadius))
        {
            if (col.tag == "Player" && Physics2D.Raycast(transform.position, col.transform.position - transform.position, (col.transform.position - transform.position).magnitude, LayerMask.GetMask("Player", "Shield")).collider.tag != "Shield")
            {

                col.GetComponent<PlayerMovement>().Knockback((col.transform.position - transform.position).normalized * knockback);

                Damager.InflictDamage(col.gameObject, Damage, damageType);

            }
        }
        yield return new WaitForSeconds(waitTime);
        Decide(true);
        yield return null;
    }

    public void Decide(bool wasAttacking) {
        if (IsCloserThan(playerDistance * (wasAttacking?1:1.2f)))
        {
            StartCoroutine(attackSeq());
        }
        else {
            GoToPlayer(playerDistance);
        }
    }
}
