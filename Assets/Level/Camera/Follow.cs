using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public float followSpeed=0.8f;

    /// <summary>
    /// cíl, který bude následován
    /// </summary>
    public GameObject target;

	// Use this for initialization
	void Start () {

    }

    private void Update()
    {
        if(followSpeed==1) transform.position = target.transform.position;
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (followSpeed != 1) transform.position = Vector3.Lerp(transform.position, target.transform.position, followSpeed);
	}

}
