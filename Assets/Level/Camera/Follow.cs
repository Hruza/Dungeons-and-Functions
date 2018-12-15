using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public float followSpeed=0.8f;

    private Vector3 target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        target = Player.player.transform.position;
        transform.position = Vector3.Lerp(transform.position, target, followSpeed);
	}

}
