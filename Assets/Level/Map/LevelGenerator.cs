using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;


public class LevelGenerator : MonoBehaviour {
    /// <summary>
    /// Bloky pouzite pri generovani, na pozici 0 je wall tile
    /// </summary>
    public GameObject[] tile;


    public int minRoomSize=3;


    /// <summary>
    /// 
    /// </summary>
    public GameObject roomObject;

    private int[,] map;

    public int tileSize=5;

    public int roomCount;
    public int roomSpread = 1;

    public EnemyProperties[] enemies;
    public int minRoomSizeVariability = 2;
    public bool debug = false;
    void Start () {
        if(debug) Generate(roomCount,enemies,1);
    }
	
	void Update () {
        
	}

    public void Generate(int roomCount,EnemyProperties[] enemies,int difficulty) {
        if (roomCount <= 0) return;
        //ToDo:Random.seed = seed;

        int totalEnemies = enemies.Length;
        int chosenEnemiesCount = 0;
        int enemyCount = 1;


        //starting room
        Room start = new Room(Vector2Int.zero, 3, 3);
        start.combat = false;

        //declare some temporary variables
        int offset = 0;
        bool intersects;
        Vector2 dir;
        Vector2 realPos;
        int lMost=-1;
        int rMost=1;
        int tMost=1;
        int bMost=-1;

        //create rooms
        List<Room> roomList = new List<Room>();
        roomList.Add(start);
        for (int i = 0; i < roomCount; i++)
        {
            int maxEnemyCount = totalEnemies - chosenEnemiesCount - (roomCount - i)+1;
            if (i + 1 == roomCount) enemyCount = maxEnemyCount;
            else if (maxEnemyCount >= 1)
            {
                enemyCount = Random.Range(1, maxEnemyCount);
            }
            else enemyCount = 0;
            //CreateVectors
            dir = Random.insideUnitCircle;
            if (dir == Vector2.zero) dir = Vector2.up;
            realPos = 5 * Random.insideUnitCircle;
            dir.Normalize();

            //Create room
            int h = minRoomSize + Random.Range(enemyCount - 1, Mathf.FloorToInt(Mathf.Sqrt(enemyCount )) + minRoomSizeVariability);
            int w = minRoomSize + Random.Range(enemyCount - 1, Mathf.FloorToInt(Mathf.Sqrt(enemyCount )) + minRoomSizeVariability);
            Room current = new Room(Vector2Int.FloorToInt(realPos), h, w);

            //move until it doesn't intersects
            offset = 0;
            int minOffset = Random.Range(roomSpread, roomSpread + 2);
            while (offset < minOffset)
            {
                realPos += dir;
                current.position = Vector2Int.FloorToInt(realPos);
                intersects = false;
                for (int j = 0; j <= i; j++)
                {
                    if (current.Intersects(roomList[j]))
                    {
                        intersects = true;
                        break;
                    }
                }
                if (!intersects) offset++;
                else offset = 0;
            }

            //addEnemies
            EnemyProperties[] chosenEnemies = new EnemyProperties[enemyCount];
                for (int k = 0; k < enemyCount; k++)
                {
                    chosenEnemies[k] = enemies[chosenEnemiesCount + k];
                    chosenEnemies[k].Level = difficulty;
                
                }
            current.enemies = chosenEnemies;
            chosenEnemiesCount += enemyCount;

            //set boundaries
            int horizontal = current.width / 2;
            int vertical = current.height/2;
            Vector2Int pos = current.position;
            if (pos.x - horizontal < lMost) lMost = pos.x - horizontal;
            if (pos.x + horizontal > rMost) rMost = pos.x + horizontal;
            if (pos.y + vertical > tMost) tMost = pos.y + vertical;
            if (pos.y - vertical < bMost) bMost = pos.y - vertical;


            //add to list
            roomList.Add(current);
        }

        //ToDo: add something interesting

        Room exit = new Room(new Vector2Int(rMost+5,0),1,1);
        rMost += 5;

        //initialize map
        Vector2Int center = new Vector2Int(-lMost+1,-bMost+1);
        int mapWidth = rMost - lMost+3;
        int mapHeight = tMost - bMost+3; ;
        map = new int[mapWidth,mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] = 0;
            }
        }

        //set rooms
        foreach (Room room in roomList)
        {
            GenerateRoom(room, center);
        }

        //connect rooms
        foreach (Room room in roomList) {
            CreatePath(room, roomList[Random.Range(0, roomCount)]);
        }

        exit.position += center;
        map[exit.position.x, exit.position.y] = 3;
        CreatePath(exit, roomList[Random.Range(0, roomCount)]);

        //generate map
        CreateMap(mapWidth,mapHeight);
        Player.player.transform.position = new Vector3(start.position.x*5,start.position.y*5,0);
    }

    private void CreatePath(Room room1,Room room2)
    {
        if (Random.value > 0.5f) {
            Room temp = room1;
            room1 = room2;
            room2 = temp;
        }
        int y = room1.position.y;
        int x=0;
        try
        {
            for (x = Mathf.Min(room1.position.x, room2.position.x); x <= Mathf.Max(room1.position.x, room2.position.x); x++)
            {
                if (map[x, y] == 0) map[x, y] = 2;
            }
            x = room2.position.x;
            for (y = Mathf.Min(room1.position.y, room2.position.y); y <= Mathf.Max(room1.position.y, room2.position.y); y++)
            {
                if (map[x, y] == 0) map[x, y] = 2;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.LogError(x.ToString() + " " + y.ToString());
            Debug.Log(map.Length);
            throw;
        }
    }

    private void CreateMap(int width,int height) {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                try
                {
                    Instantiate(tile[map[i, j]], new Vector3(5 * i, 5 * j, tile[map[i, j]].transform.position.z), transform.rotation, transform);
                }
                catch (System.IndexOutOfRangeException) {
                    Debug.LogError(i.ToString() + " " + j.ToString());
                    Debug.Log(map.Length);
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Vygeneruje jednu mistnost
    /// </summary>
    /// <param name="position">Pozoce stredu mistnosti</param>
    /// <param name="width">Sirka mistnosti</param>
    /// <param name="height">Vyska mistnosti</param>
    /// <param name="enemies">Enemies v mistnosti</param>
    void GenerateRoom(Room room,Vector2Int center)
    {
        room.position += center;
        int x;
        int y;
        for (int i = 0; i < room.width; i++)
        {
            x = room.position.x - (room.width / 2) + i;
            for (int j = 0; j < room.height; j++)
            {
                y = room.position.y - (room.height / 2) + j;

                try
                {
                    map[x, y] = 1;
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogError(x.ToString()+" "+y.ToString());
                    throw;
                }
            }
        }

        if (room.combat)
        {
            Vector3 pos = new Vector3(5 * room.position.x, 5 * room.position.y, 0);
            if (room.width % 2 == 0) pos.x -= 2.5f;
            if (room.height % 2 == 0) pos.y -= 2.5f;
            GameObject roomObj = (GameObject)Instantiate(roomObject, pos, transform.rotation, transform);
            roomObj.GetComponent<RoomController>().Initialize(room.width, room.height);
            roomObj.GetComponent<RoomController>().EnemiesToSpawn= room.enemies;
        }
    }
}

class Room {
    public bool combat = true;
    public int width;
    public int height;
    public Vector2Int position;
    public EnemyProperties[] enemies;

    public Room(Vector2Int position,int height,int width) {
        this.position = position;
        this.height = height;
        this.width = width;
    }

    public bool Intersects(Room other) {
        Vector2Int dif = position - other.position;
        if ((((other.width + width) / 2)+1 >= Mathf.Abs(dif.x)) && (((other.height + height) / 2)+1>= Mathf.Abs(dif.y))) return true;
        else return false;
    }
}