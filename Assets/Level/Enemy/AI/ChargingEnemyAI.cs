using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyAI : EnemyAI
{
    public float playerDistance = 5f;

    /// <summary>
    /// Doba mezi jednotlivymi utoky
    /// </summary>
    public float attackDelay = 0.5f;

    public float waitTime = 0.8f;

    public float chargeSpeed=4;

    /// <summary>
    /// Jak dlouho trvá, než se aplikuje damage
    /// </summary>
    public float attackDuration = 0.7f;

    public float knockback = 1f;

    public override void Start()
    {
        base.Start();
        GoToPlayer(playerDistance);
    }

    protected override void WalkFinished()
    {
        Decide(false);
    }
    private bool attacking = false;
    protected virtual IEnumerator attackSeq()
    {
        anim.SetTrigger("Attack");
        Vector3 dir = target.transform.position - transform.position;
        yield return new WaitForSeconds(attackDelay);
        collided = new List<GameObject>();
        attacking = true;
        Nav.Dash(dir, chargeSpeed);
        yield return new WaitForSeconds(attackDuration);
        attacking = false;
        yield return new WaitForSeconds(waitTime);
        Decide(true);
        yield return null;
    }

    List<GameObject> collided;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (attacking) {
            GameObject col = collision.gameObject;
            if (col.tag == "Player" && !collided.Contains(col) && Physics2D.Raycast(transform.position, col.transform.position - transform.position, (col.transform.position - transform.position).magnitude, LayerMask.GetMask("Player", "Shield")).collider.tag != "Shield")
            {
                collided.Add(col);
                col.GetComponent<PlayerMovement>().Knockback((col.transform.position - transform.position).normalized * knockback);

                Damager.InflictDamage(col.gameObject, Damage, (col.transform.position - transform.position).normalized, damageType);

            }
        }
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
