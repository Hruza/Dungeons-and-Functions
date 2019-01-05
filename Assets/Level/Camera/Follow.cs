using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public float followSpeed=0.8f;

    /// <summary>
    /// cíl, který bude následován
    /// </summary>
    private Vector3 target;

	// Use this for initialization
	void Start () {
        target = Player.player.transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        transform.position = Vector3.Lerp(transform.position, target, followSpeed);
	}

}
