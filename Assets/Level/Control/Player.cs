using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
   
    /// <summary>
    /// Aktualni hrac, typ GameObject
    /// </summary>
    static public GameObject player;

    static public Rigidbody2D rbody;

	void Start () {
        player = this.gameObject;
        rbody = GetComponent<Rigidbody2D>();
	}
}
