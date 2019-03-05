﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : NPC
{
    /// <summary>
    /// Jak blizko musi byt k hraci, aby zautocil
    /// </summary>
    public float playerDistance = 2f;
    private enum State { moving,attacking,waiting };
    private State state;

    /// <summary>
    /// Jak daleko musze byt hrac, aby dostal damage
    /// </summary>
    public float attackRadius=1.5f;

    /// <summary>
    /// Jak dlouho po zastaveni zacne enemy utocit
    /// </summary>
    public float timeToStartAttacking= 0.2f;

    /// <summary>
    /// Doba mezi jednotlivymi utoky
    /// </summary>
    public float attackDelay = 0.5f;

    /// <summary>
    /// Jak dlouho trvá, než se aplikuje damage
    /// </summary>
    public float attackDuration = 0.7f;

    public float knockback=1f;

    private GameObject player;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>(); 
        player = Player.player;
        state = State.moving;
        GoToTarget(player, playerDistance);
    }


    protected override void WalkStarted()
    {
        anim.SetBool("isWalking", isWalking);
    }

    protected override void WalkEnded()
    {
        anim.SetBool("isWalking", isWalking);
        Decide();
    }

    protected virtual IEnumerator attackSeq() {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, attackRadius))
        {
            if (col.tag == "Player")
            {
                col.GetComponent<PlayerMovement>().Knockback((col.transform.position - transform.position)*knockback);
                col.SendMessage("GetDamage", Damage);
            }
        } 
        Decide();
        yield return null;
    }

    private void MoveAround()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.z = 0;
        float temp = dir.x;
        dir.x = -dir.y;
        dir.y = temp;
        if (Random.Range(0, 2) == 1) dir *= -1;
        dir = dir.normalized * 4;
        GoToTarget(transform.position + dir);
    }

    private void Attack()
    {
        StartCoroutine(attackSeq());
    }

    protected void Decide()
    {
        Debug.Log("Deciding");
        bool playerIsClose = (player.transform.position - transform.position).sqrMagnitude < playerDistance * playerDistance * 2;
        switch (state)
        {
            case State.attacking:
                if (playerIsClose)
                {
                    state = State.waiting;
                    Invoke("Decide", attackDelay);
                }
                else {
                    state = State.moving;
                    GoToTarget(player, playerDistance);
                }
                break;

            case State.moving:
                if (playerIsClose)
                {
                    state = State.attacking;
                    Invoke("Attack", timeToStartAttacking);
                }
                else
                {
                    state = State.moving;
                    MoveAround();
                }
                break;

            case State.waiting:
                if (playerIsClose)
                {
                    state = State.attacking;
                    Attack();
                }
                else {
                    state = State.moving;
                    GoToTarget(player, playerDistance);
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
