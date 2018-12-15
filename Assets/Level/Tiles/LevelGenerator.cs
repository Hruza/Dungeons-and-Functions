using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GameObject[] tile;
    public GameObject wall;
    private GameObject spawnedTile;

    public int tileSize=5;
	// Use this for initialization
	void Start () {
        this.runInEditMode = true;
    }
	
	// Update is called once per frame
	void Update () {
        
	}   
		
    public void Generate(int n, int m) {
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if(Mathf.PerlinNoise(10f*i/n,10f*j/m)>0.4)
                    spawnedTile = (GameObject)Instantiate(tile[0], transform.position+(5*Vector3.up*j)+(5 * Vector3.right * i)+Vector3.forward, transform.rotation);
                else
                    spawnedTile = (GameObject)Instantiate(wall, transform.position + (5 * Vector3.up * j) + (5 * Vector3.right * i) + Vector3.forward, transform.rotation);
                spawnedTile.transform.SetParent(transform);
            }
        }
    }
}
