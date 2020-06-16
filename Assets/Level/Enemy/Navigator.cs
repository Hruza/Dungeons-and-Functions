using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Navigator : MonoBehaviour
{
    protected Animator anim;
    public float defaultTargetTolerance=1f;
    protected Rigidbody2D rb;
    private EnemyAI AI;
    public enum Avoidance {none, avoidNearest};
    public Avoidance obstacleAvoidance=Avoidance.avoidNearest;

    public enum WalkingOutput {success, obstacleDetected, gaveUp, timeUp };

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
            AI.SendMessage("WalkEnded",output);
        else
            Debug.Log("unassigned AI");
    }
}
