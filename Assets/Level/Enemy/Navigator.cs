using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Navigator : MonoBehaviour
{

    protected float defaultTargetTolerance=1f;
    protected Rigidbody2D rb;
    public EnemyAI AI;
    public enum Avoidance {none, avoidNearest};
    public Avoidance obstacleAvoidance=Avoidance.avoidNearest;

    public enum WalkingOutput {success, obstacleDetected, gaveUp };

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
            defaultTargetTolerance = Mathf.Max(coll.bounds.extents.x,coll.bounds.extents.y) * 2.5f;
        else
            defaultTargetTolerance = 2f;
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

    virtual public void GoToTarget(GameObject target, float tolerance)
    {
        Debug.LogWarning("Metoda GoToTarget není definována");
    }

    virtual public void GoToTarget(Vector3 target)
    {
        GoToTarget(target, defaultTargetTolerance);
    }

    virtual public void GoToTarget(Vector3 target, float tolerance)
    {
        Debug.LogWarning("Metoda GoToTarget není definována");
    }

    protected void SendOutput(WalkingOutput output) {
        if (AI != null)
            AI.SendMessage("WalkEnded",output);
        else
            Debug.Log("unassigned AI");
    }
}
