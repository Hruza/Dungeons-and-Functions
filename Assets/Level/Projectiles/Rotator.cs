using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationTime=1;
    public AnimationCurve rotationSpeed=AnimationCurve.Constant(0,1,1);
    public float speedMultiplicator=1;

    private float startTime;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        this.transform.Rotate(Vector3.forward, speedMultiplicator * rotationSpeed.Evaluate(Mathf.Clamp(Time.realtimeSinceStartup-startTime/rotationTime,0,1)));
    }
}
