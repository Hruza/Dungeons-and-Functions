using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GameObject[] tile;
    public GameObject[] walls;

    /// <summary>
    /// 
    /// </summary>
    public GameObject roomObject;

    private GameObject spawnedTile;

    private int[,] map;
    private GameObject rooms;

    public int tileSize=5;

    void Start () {
      //  Generate(50, 50);
    }
	
	void Update () {
        
	}   
		
    public void Generate(int width, int height) {

        //ToDo: odstranit
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if(Mathf.PerlinNoise(10f*i/width,10f*j/height)>0.4)
                    spawnedTile = (GameObject)Instantiate(tile[0], transform.position+(5*Vector3.up*j)+(5 * Vector3.right * i)+Vector3.forward, transform.rotation);
                else
                    spawnedTile = (GameObject)Instantiate(walls[0], transform.position + (5 * Vector3.up * j) + (5 * Vector3.right * i) + Vector3.forward, transform.rotation);
                spawnedTile.transform.SetParent(transform);
            }
        }

        map = new int[width,height];
        //ToDo: Insert legit code

    }


    /// <summary>
    /// Vygeneruje jednu mistnost
    /// </summary>
    /// <param name="position">Pozoce stredu mistnosti</param>
    /// <param name="width">Sirka mistnosti</param>
    /// <param name="height">Vyska mistnosti</param>
    /// <param name="enemies">Enemies v mistnosti</param>
    GameObject GenerateRoom(Vector3 position,int width, int height,EnemyProperties[] enemies)
    {
        //ToDo: Generating tiles

        GameObject room = (GameObject)Instantiate(roomObject, position, transform.rotation);
        room.GetComponent<Room>().Initialize(width,height);
        return room;
    }
}
