﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomController : MonoBehaviour {
    const int tileSize = 5;
    int enemyCount;
    public GameObject[] triggerOnClear;
    public GameObject[] triggerOnEnter;
    List<GameObject> livingEnemies;
    Vector2 dimensions;
    
    //todo: Public jen pro testovani
    public EnemyProperties[] enemiesToSpawn;
    public EnemyProperties[] EnemiesToSpawn {
        private get {
            return enemiesToSpawn;
        }
        set {
            if (entered) Debug.LogError("Adding enemeies in entered room " + this.ToString());
            else enemiesToSpawn = value;
        }
    }
    
    bool entered = false;
    bool initiated=false;
    BoxCollider2D roomCollider;

    /// <summary>
    /// Only for manual debugging, on true: room collider in not set to room size.
    /// </summary>
    public bool selfInitialize = false;

	
	void Start () {
        

        if(selfInitialize) Initialize(1, 1);
        
	}

    /// <summary>
    /// Inicializuje mistnost s danou velikosti, tato operave musi byt provedena pred pouzitim mistnosti
    /// </summary>
    /// <param name="x">Sirka mistnost</param>
    /// <param name="y">Vyska mistnosti</param>
    public void Initialize(int x,int y)
    {
        initiated = true;
        livingEnemies = new List<GameObject>();

        roomCollider = GetComponent<BoxCollider2D>();
        enemyCount = 0;

        dimensions.x = x;
        dimensions.y = y;

        if (!selfInitialize) roomCollider.size = new Vector2(tileSize * x - 0.5f, tileSize * y - 0.5f);
        else {
            dimensions = roomCollider.size/5;
            }
    }

    /// <summary>
    /// Spawne vsechny enemies v listu enemiesToSpawn, pozice je nahodna v ramci mistnosti.
    /// </summary>
    void SpawnEnemies() {
        foreach (EnemyProperties enemy in enemiesToSpawn)
        {
            enemyCount++;
            Vector3 randPos = new Vector3(( Random.value - 0.5f) * (dimensions.x - 0.1f) * tileSize, (Random.value - 0.5f) * (dimensions.y - 0.1f) * tileSize);
            GameObject currentEnemy = (GameObject)Instantiate(enemy.EnemyGameObject, transform.position + randPos, transform.rotation);
            currentEnemy.GetComponent<NPC>().Initialize(enemy.Level);
            livingEnemies.Add(currentEnemy);
        }
    }


	// Update is called once per frame
	void Update () {

    }

    IEnumerator CheckCleared() {

        while (livingEnemies.Exists(i => i!=null)
            // alternativa: GameObject.FindGameObjectWithTag("enemy")==null
            )
        {
            yield return new WaitForSeconds(0.5f);
            
            
        }

        foreach (GameObject onClearObject in triggerOnClear)
        {
            onClearObject.SendMessage("OnClear");
        }


        Debug.Log("You are murderer!!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!entered) {
            if (!initiated && !selfInitialize) Debug.LogError("Room " + this.ToString() + " is not initialized");

            foreach (GameObject onEnterObject in triggerOnEnter)
            {
                onEnterObject.SendMessage("OnEnter");
            }

            //Debug.Log(enemiesToSpawn);
            entered = true;
            SpawnEnemies();

            StartCoroutine(CheckCleared());
        }

    }
}