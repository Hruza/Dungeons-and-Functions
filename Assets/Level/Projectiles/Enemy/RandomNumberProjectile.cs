using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberProjectile : MonoBehaviour {
    public TMPro.TextMeshPro cislo;
	// Use this for initialization
	void Start () {
        cislo.text = Random.Range(0, 10).ToString();
	}
}
