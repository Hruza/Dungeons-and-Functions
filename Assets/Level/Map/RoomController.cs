using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomController : MonoBehaviour {

    int enemyCount;

    public int wave=5;
    public int maxWave=10;


    public GameObject[] triggerOnClear;
    public GameObject[] triggerOnEnter;

    public bool spawnAllInCenter = false;

    public GameObject summoner;

    List<GameObject> livingEnemies;
    Vector2 dimensions;

    public Transform[] spawns;
    
    //todo: Public jen pro testovani
    private List<EnemyProperties> enemiesToSpawn;
    public List<EnemyProperties> EnemiesToSpawn {
        get {
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
        if (spawns.Length == 0) {
           // spawns = transform.GetChild(0);
                }
        if(selfInitialize) Initialize(1, 1);
        enemyCount = 0;
        livingEnemies = new List<GameObject>();
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

        if (!selfInitialize) roomCollider.size = new Vector2(LevelGenerator.tileSize * x - 1.5f, LevelGenerator.tileSize * y - 1.5f);
        else {
            dimensions = roomCollider.size/ LevelGenerator.tileSize;
            }
    }

    int cap;

    /// <summary>
    /// Spawne vsechny enemies v listu enemiesToSpawn, pozice je nahodna v ramci mistnosti.
    /// </summary>
    void SpawnEnemies() {
        if (enemiesToSpawn.Count - enemyCount > maxWave) cap = enemyCount + wave;
        else cap = enemyCount + wave+1;
        for (int i = enemyCount; i < enemiesToSpawn.Count; i++)
        {
            EnemyProperties enemy = enemiesToSpawn[i]; 
            enemyCount++;
            Vector3 randPos;
            if (spawnAllInCenter) randPos = transform.position;
            else if (spawns.Length==0) {
                randPos = transform.GetChild(i % transform.childCount ).position;
            }
            else {
                randPos = spawns[i % spawns.Length].position;
            }
            GameObject currentEnemy = (GameObject)Instantiate(enemy.EnemyGameObject, randPos, Quaternion.identity);
            currentEnemy.SetActive(false);
            NPC npc = currentEnemy.GetComponent<NPC>();
            if(npc!=null)
                npc.Initialize(enemy);
            else
                currentEnemy.GetComponent<EnemyAI>().Initialize(enemy);
            GameObject particles = (GameObject)Instantiate(summoner, randPos, transform.rotation);
            particles.GetComponent<Summoner>().enemy = currentEnemy;
            livingEnemies.Add(currentEnemy);
            if (enemyCount >= cap) break;
        }
    }


	// Update is called once per frame
	void Update () {

    }

    IEnumerator CheckCleared() {
        bool cleared = false;
        while (!cleared) {
            while (livingEnemies.Exists(i => i != null))
            // alternativa: GameObject.FindGameObjectWithTag("enemy")==null
            {
                yield return new WaitForSeconds(0.5f);
            }
            if (enemyCount == enemiesToSpawn.Count) cleared = true;
            else {
                SpawnEnemies();
            }
        }
        LevelController.levelController.RoomCleared();

        foreach (GameObject onClearObject in triggerOnClear)
        {
            onClearObject.SendMessage("OnClear");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!entered) {

            foreach (GameObject onEnterObject in triggerOnEnter)
            {
                onEnterObject.SendMessage("OnEnter");
            }

            //Debug.Log(enemiesToSpawn);
            entered = true;
            if (enemiesToSpawn.Count > 0)
            {
                SpawnEnemies();

                StartCoroutine(CheckCleared());
            }
            else {
                LevelController.levelController.RoomCleared();
                foreach (GameObject onClearObject in triggerOnClear)
                {
                    onClearObject.SendMessage("OnClear");
                }
            }
        }

    }
}
