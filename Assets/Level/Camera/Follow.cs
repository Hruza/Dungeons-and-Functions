using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public float followSpeed=0.8f;

    /// <summary>
    /// cíl, který bude následován
    /// </summary>
    private GameObject target;

	// Use this for initialization
	void Start () {
        target = Player.player;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        transform.position = Vector3.Lerp(transform.position, target.transform.position, followSpeed);
	}

}
