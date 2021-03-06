﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Navigator : MonoBehaviour
{
    protected Animator anim;
    public float defaultTargetTolerance=1f;
    private Rigidbody2D rb;
    protected Rigidbody2D RB {
        get {
            if (rb == null) {
                rb = GetComponent<Rigidbody2D>();
            }
            return rb;
        }
        set {
            rb = value;
        }
    }
    private EnemyAI AI;
    public enum Avoidance {none, avoidNearest};
    public Avoidance obstacleAvoidance=Avoidance.avoidNearest;

    public enum WalkingOutput {success, obstacleDetected, gaveUp, timeUp };

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        Collider2D coll = GetComponent<Collider2D>();
        AI = GetComponent<EnemyAI>();
    }


    protected bool IsInLineOfSight(GameObject tgt)
    {
        return IsInLineOfSight(tgt.transform.position);
    }

    protected bool IsInLineOfSight(Vector3 tgt) {
        Vector3 dif = tgt - transform.position;
        if (Physics2D.Raycast(transform.position,dif ,dif.magnitude, LayerMask.GetMask("Map")))
        {
            return false;
        }
        return true;
    }

    protected float speedCoefficient=1;
    
    public void DbfSlowness(float time,float coefficient=0.6f) {
        speedCoefficient = coefficient;
        CancelInvoke("ResetSpeedCoefficient");
        Invoke("ResetSpeedCoefficient",time);
    }

    private void ResetSpeedCoefficient() {
        speedCoefficient = 1;
    }

    virtual public void GoToTarget(GameObject target)
    {
        GoToTarget(target, defaultTargetTolerance);
    }

    virtual public void GoToTarget(GameObject target, float tolerance,float speedModifier=1, float timeLimit=0)
    {
        Debug.LogWarning("Metoda GoToTarget není definována");
    }

    virtual public void GoToTarget(Vector3 target)
    {
        GoToTarget(target, defaultTargetTolerance);
    }

    virtual public void GoToTarget(Vector3 target, float tolerance, bool callback=true,float speedModifier = 1, float timeLimit = 0)
    {
        Debug.LogWarning("Metoda GoToTarget není definována");
    }

    virtual public void Dash(Vector2 direction, float speed) {
        Debug.LogWarning("Metoda GoToTarget není definována");
    }

    virtual public void Stop() {
        Debug.LogWarning("Metoda Stop není definována");
    }

    protected void SendOutput(WalkingOutput output) {
        if (AI != null)
            AI.SendMessage("WalkEnded", output);
        else
        {
         //   Debug.Log("unassigned AI");
            this.gameObject.SendMessage("WalkEnded", output);
        }
    }
}
