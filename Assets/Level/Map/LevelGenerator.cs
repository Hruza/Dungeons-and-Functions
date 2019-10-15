using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;


public class LevelGenerator : MonoBehaviour {

    public RoomPatternRepository repo;
    /// <summary>
    /// Bloky pouzite pri generovani, na pozici 0 je wall tile
    /// </summary>
    public GameObject[] tile;

    public GameObject door;

    public GameObject treasure;

    public int minRoomSize=3;


    /// <summary>
    /// 
    /// </summary>
    public GameObject roomObject;

    private int[,] map;

    static public int tileSize=4;

    public int roomCount;
    public int roomSpread = 1;

    public EnemyProperties[] enemies;
    public int minRoomSizeVariability = 2;
    public bool debug = false;
    void Start () {
    }
	
	void Update () {
        
	}

    public enum GeneratorPreset { normal, vast, boss };

    public enum RoomConnectionPreset { random, jarnik, addShortUnillAll}

    public struct Boundaries{
        public int r;
        public int l;
        public int t;
        public int b;
    }

    private Boundaries most;

    private Vector2Int center;

    public void Generate(Level level)
    { 
        int roomCount=level.roomCount;
        EnemyProperties[] enemies = EnemyBundle.Merge(level.enemies);
        int difficulty = level.difficulty;
        GeneratorPreset preset = level.generatorPreset;
        RoomConnectionPreset connections = level.roomConnections;

        if (roomCount <= 0) return;
        this.roomCount = roomCount;
        //ToDo:Random.seed = seed;

        int totalEnemies = enemies.Length;
        int chosenEnemiesCount = 0;
        int enemyCount = 1;


        //starting room
        Room start = new StartingRoom();

        //declare some temporary variables
        most.l=-1;
        most.r=1;
        most.t=1;
        most.b=-1;

        //create rooms
        List<Room> roomList = new List<Room> { start };
        int createdRoomsCount=1;

        for (; createdRoomsCount <= roomCount; createdRoomsCount++)
        {
            int maxEnemyCount = totalEnemies - chosenEnemiesCount - (roomCount - createdRoomsCount);
            if (createdRoomsCount == roomCount) enemyCount = maxEnemyCount;
            else if (maxEnemyCount >= 1)
            {
                enemyCount = Random.Range(1, Mathf.Clamp(Mathf.FloorToInt(maxEnemyCount/(roomCount - createdRoomsCount))*2,1,maxEnemyCount));
            }
            else enemyCount = 0;


            //Create room
 /*           int dim = Mathf.FloorToInt(Mathf.Sqrt(enemyCount));
            int h = minRoomSize + Random.Range(dim - 1, dim + minRoomSizeVariability);
            int w = minRoomSize + Random.Range(dim - 1, dim + minRoomSizeVariability);
            if (preset == GeneratorPreset.boss) {
                h += 3;
                w += 3;
            }
            Room current = new Room( h, w, roomSpread, roomList);
            */

            //addEnemies

            EnemyProperties[] chosenEnemies = new EnemyProperties[enemyCount];
                for (int k = 0; k < enemyCount; k++)
                {
                    chosenEnemies[k] = enemies[chosenEnemiesCount + k];
                    chosenEnemies[k].Level = difficulty;
                
                }
            chosenEnemiesCount += enemyCount;

            Room current = new Room(repo,chosenEnemies, roomSpread, roomList, preset);

            //set boundaries
            SetBoundaries(current);

            //add to list
            roomList.Add(current);
        }

        Room exit;
        switch (preset)
        {
            case GeneratorPreset.vast:
                exit = new ExitRoom(most);
                break;
            case GeneratorPreset.normal:
                exit = new ExitRoom(Random.insideUnitCircle,roomList,2);
                break;
            case GeneratorPreset.boss:
                exit = new ExitRoom(roomList[1].position-roomList[0].position,roomList,roomSpread);
                break;
            default:
                exit = new ExitRoom(most);
                break;
        }


        SetBoundaries(exit);
        roomList.Add(exit);
        //ToDo: add something interesting

        List<Room> treasures = new List<Room>();
        foreach (SecretRoom secret in level.secretRooms)
        {
            TreasureRoom current = new TreasureRoom(Random.insideUnitCircle, roomList, 1, secret);
            SetBoundaries(current);
            treasures.Add(current);
        }
     /*   foreach (SecretRoom secret in level.secretRooms)
        {
            
        }
        */
        //initialize map
        center = new Vector2Int(-most.l+1,-most.b+1);
        int mapWidth = most.r - most.l+3;
        int mapHeight = most.t - most.b+3; ;
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
            room.GenerateRoomToMap(center, ref map);
        }
        foreach (Room room in treasures)
        {
            room.GenerateRoomToMap(center, ref map);
        }
        //connect rooms
        ConnectRooms(roomList,treasures, connections);

        foreach(Room room in roomList)
        { 
            switch (room.roomType)
            {
                case Room.RoomType.combat:
                    PlaceDoors(room);
                    PlaceRoomControllerObject(room);
                    break;
                case Room.RoomType.exit:
                    break;
                case Room.RoomType.start:
                    break;
                default:
                    break;

            }

        }
        foreach (Room room in treasures)
        {
            PlaceTreasure(room);
        }


        //generate map
        CreateMap(mapWidth,mapHeight);
        Player.player.transform.position = new Vector3(start.position.x*tileSize,start.position.y*tileSize,0);
    }

    private void ConnectRooms(List<Room> roomList,List<Room> secrets,RoomConnectionPreset connections) {
        switch (connections)
        {
            case RoomConnectionPreset.random:
                foreach (Room room in roomList)
                {
                    int i = 0;
                    Room random;
                    do
                    {
                        switch (room.roomType)
                        {
                            case Room.RoomType.combat:
                                random = roomList[Random.Range(0, roomCount - 1)];
                                break;
                            case Room.RoomType.start:
                            case Room.RoomType.exit:
                                random = roomList[Random.Range(1, roomCount - 1)];
                                break;
                            case Room.RoomType.treasure:
                                random = roomList[Random.Range(0, roomCount - 1)];
                                break;
                            default:
                                random = roomList[Random.Range(0, roomCount - 1)];
                                break;
                        }
                        i++;
                    }
                    while ((random == room || random.moreThanOneConnections == false) && i < 100);
                    CreatePath(room, random);
                }
                break;
            case RoomConnectionPreset.jarnik:
            case RoomConnectionPreset.addShortUnillAll:
                bool[,] connectionMatrix = new bool[roomList.Count, roomList.Count];
                List<int> connected = new List<int>{0};
                while (connected.Count<roomList.Count)
                {
                    int from= 0;
                    int to=0;
                    int min = 100000;
                    int dist;
                    for (int i = 0; i < connected.Count; i++)
                    {
                        for (int j = 0; j < roomList.Count; j++)
                        {
                            if (i!=j && !connectionMatrix[i, j] && ( ( connections==RoomConnectionPreset.jarnik && !connected.Contains(j) ) || connections==RoomConnectionPreset.addShortUnillAll) ) {
                                if ((!connected.Contains(j) || roomList[j].moreThanOneConnections) && (!connected.Contains(j) || roomList[i].moreThanOneConnections)) {
                                    dist = Room.manhattanMetric(roomList[i], roomList[j]);
                                    if (dist < min) {
                                        min = dist;
                                        from = i;
                                        to = j;
                                    }
                                }
                            }
                        }
                    }
                    if(!connected.Contains(to)) connected.Add(to);
                    connectionMatrix[from, to] = true;
                    connectionMatrix[to, from] = true;
                    CreatePath(roomList[from], roomList[to]);
                }
                
                    break;
            default:
                break;
        }
        foreach (Room secret in secrets)
        {
            int min = 100000;
            int dist;
            int from = 1;
            for (int i = 0; i < roomList.Count - 1; i++)
            {
                dist = Room.manhattanMetric(secret, roomList[i]);
                if (dist < min)
                {
                    from = i;
                    min = dist;
                }
            }
            CreatePath(roomList[from], secret, true);
        }
    }

    private void SetBoundaries(Room room,int offset=1) {
        int horizontal = room.width / 2;
        int vertical = room.height / 2;
        Vector2Int pos = room.position;
        if (pos.x - horizontal < most.l) most.l = pos.x - horizontal-offset;
        if (pos.x + horizontal > most.r) most.r = pos.x + horizontal+offset;
        if (pos.y + vertical > most.t) most.t = pos.y + vertical+offset;
        if (pos.y - vertical < most.b) most.b = pos.y - vertical-offset;
    }

    private void CreatePath(Room room1,Room room2,bool secret=false)
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
                if (map[x, y] == 0) map[x, y] = secret?4:2;
            }
            x = room2.position.x;
            for (y = Mathf.Min(room1.position.y, room2.position.y); y <= Mathf.Max(room1.position.y, room2.position.y); y++)
            {
                if (map[x, y] == 0) map[x, y] = secret ? 4 : 2;
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
                    GameObject spawnedTile = (GameObject)Instantiate(tile[map[i, j]], new Vector3(tileSize * i, tileSize * j, tile[map[i, j]].transform.position.z), transform.rotation, transform);
                    spawnedTile.transform.localScale = new Vector3(tileSize,tileSize,spawnedTile.transform.localScale.z);
                }
                catch (System.IndexOutOfRangeException) {
                    Debug.LogError(i.ToString() + " " + j.ToString());
                    Debug.Log(map.Length);
                    throw;
                }
            }
        }
    }



    private void PlaceRoomControllerObject(Room room) {
        Vector3 pos = new Vector3(tileSize * room.position.x, tileSize * room.position.y, 0);
        if (room.width % 2 == 0) pos.x -= tileSize / 2f;
        if (room.height % 2 == 0) pos.y -= tileSize / 2f;
        GameObject roomObj = (GameObject)Instantiate(roomObject, pos, transform.rotation, transform);
        RoomController roomController = roomObj.GetComponent<RoomController>();
        roomController.Initialize(room.width, room.height);
        roomController.Initialize(room.width, room.height);
        roomController.EnemiesToSpawn = room.enemies;
        roomController.triggerOnEnter = room.onEnter.ToArray();
        roomController.triggerOnClear = room.onClear.ToArray();
    }

    private void PlaceDoors(Room room) {
        int x;
        int y;
        for (int i = 0; i < room.width; i++)
        {
            x = room.position.x - (room.width / 2) + i;
            y = room.position.y - (room.height / 2) + room.height;
            if (map[x, y] == 2) {
                GameObject doorTile = (GameObject)Instantiate(door, new Vector3(tileSize*x, tileSize*y, door.transform.position.z), transform.rotation);
                room.onClear.Add(doorTile);
                room.onEnter.Add(doorTile);
            }
        }
        for (int i = 0; i < room.width; i++)
        {
            x = room.position.x - (room.width / 2) + i;
            y = room.position.y - (room.height / 2) -1;
            if (map[x, y] == 2)
            {
                GameObject doorTile = (GameObject)Instantiate(door, new Vector3(tileSize * x, tileSize * y, door.transform.position.z), transform.rotation);
                room.onClear.Add(doorTile);
                room.onEnter.Add(doorTile);
            }
        }
        for (int i = 0; i < room.height; i++)
        {
            x = room.position.x - (room.width / 2) + room.width;
            y = room.position.y - (room.height / 2) + i;
            if (map[x, y] == 2)
            {
                GameObject doorTile = (GameObject)Instantiate(door, new Vector3(tileSize * x, tileSize * y, door.transform.position.z), transform.rotation);
                room.onClear.Add(doorTile);
                room.onEnter.Add(doorTile);
            }
        }
        for (int i = 0; i < room.height; i++)
        {
            x = room.position.x - (room.width / 2) -1;
            y = room.position.y - (room.height / 2) +i;
            if (map[x, y] == 2)
            {
                GameObject doorTile = (GameObject)Instantiate(door, new Vector3(tileSize * x, tileSize * y, door.transform.position.z), transform.rotation);
                room.onClear.Add(doorTile);
                room.onEnter.Add(doorTile);
            }
        }
    }

    private void PlaceTreasure(Room room) {
        Vector3 pos = new Vector3(tileSize * room.position.x, tileSize * room.position.y, 0.5f);
        GameObject treasureObject = (GameObject)Instantiate(treasure,pos,transform.rotation);
        treasureObject.GetComponent<Interactable>().Secret = ((TreasureRoom)room).secret;
    }
}

class Room {
    public enum RoomType { combat,exit,treasure,start};
    public RoomType roomType;
    public bool moreThanOneConnections = true;
    public int width;
    public int height;
    public Vector2Int position;
    public EnemyProperties[] enemies;
    protected int[,] plan;

    public List<GameObject> onEnter;
    public List<GameObject> onClear;

    protected Room() { }

    static public int manhattanMetric(Room r1, Room r2) {
        return Mathf.Abs(r1.position.x - r2.position.x) + Mathf.Abs(r1.position.y - r2.position.y);
    }

    protected Room(Vector2Int position,int height,int width,RoomType roomType=RoomType.combat) {
        this.position = position;
        this.height = height;
        this.width = width;
        this.roomType = roomType;
    }

    /// <summary>
    /// Vytvori mistnost a posune ji tak, aby nekolidovala s ostatními
    /// </summary>
    /// <param name="position"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="others"></param>
    public Room(int height, int width, int roomSpread, List<Room> others, RoomType roomType = RoomType.combat) {
        this.height = height;
        this.width = width;
        this.roomType = roomType;

        this.position = Vector2Int.FloorToInt(5* Random.insideUnitCircle.normalized); 

        Deintersect(roomSpread, others);
    }

    public Room(RoomPatternRepository repo,EnemyProperties[] enemies, int roomSpread, List<Room> others,LevelGenerator.GeneratorPreset preset, RoomType roomType = RoomType.combat)
    {
        RoomPattern pattern;
        switch (preset)
        {
            case LevelGenerator.GeneratorPreset.normal:
                pattern = repo.GetRandomPattern(enemies.Length);
                break;
            case LevelGenerator.GeneratorPreset.vast:
                pattern = repo.GetRandomPattern(enemies.Length);
                break;
            case LevelGenerator.GeneratorPreset.boss:
                pattern = repo.GetRandomPattern(enemies.Length,"Boss");
                break;
            default:
                pattern = repo.GetRandomPattern(enemies.Length);
                break;
        }
        height = pattern.height;
        width = pattern.width;
        this.roomType = roomType;
        plan = pattern.roomPlan;
        this.enemies = enemies;
        this.position = Vector2Int.FloorToInt(5 * Random.insideUnitCircle.normalized);

        onEnter = new List<GameObject>();
        onClear = new List<GameObject>();

        Deintersect(roomSpread, others);

    }

    /// <summary>
    /// Vygeneruje jednu mistnost
    /// </summary>
    /// <param name="position">Pozoce stredu mistnosti</param>
    /// <param name="width">Sirka mistnosti</param>
    /// <param name="height">Vyska mistnosti</param>
    /// <param name="enemies">Enemies v mistnosti</param>
    public virtual void GenerateRoomToMap( Vector2Int center, ref int[,] map)
    {
        position += center;
        int x;
        int y;
        for (int i = 0; i < width; i++)
        {
            x = position.x - (width / 2) + i;
            for (int j = 0; j < height; j++)
            {
                y = position.y - (height / 2) + j;

                try
                {
                    map[x, y] = plan[i,j];
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogError(x.ToString() + " " + y.ToString());
                    throw;
                }
            }
        }
    }

    public bool Intersects(Room other) {
        Vector2Int dif = position - other.position;
        if ((((other.width + width) / 2)+1 >= Mathf.Abs(dif.x)) && (((other.height + height) / 2)+1>= Mathf.Abs(dif.y))) return true;
        else return false;
    }

    public void Deintersect(int roomSpread, List<Room> others)
    {
        Vector2 dir = Random.insideUnitCircle;
        if (dir == Vector2.zero) dir = Vector2.up;
        Vector2 realPos = position;
        dir.Normalize();
        bool intersects;
        int offset = 0;
        int minOffset = Random.Range(roomSpread, roomSpread + 2);
        while (offset < minOffset)
        {
            realPos += dir;
            position = Vector2Int.FloorToInt(realPos);
            intersects = false;
            foreach(Room other in others)
            {
                if (Intersects(other))
                {
                    intersects = true;
                    break;
                }
            }
            if (!intersects) offset++;
            else offset = 0;
        }
    }

    public void Deintersect(int roomSpread, List<Room> others, Vector2 dir)
    {
        if (dir == Vector2.zero) dir = Vector2.up;
        Vector2 realPos = position;
        dir.Normalize();
        bool intersects;
        int offset = 0;
        int minOffset = Random.Range(roomSpread, roomSpread + 2);
        while (offset < minOffset)
        {
            realPos += dir;
            position = Vector2Int.FloorToInt(realPos);
            intersects = false;
            foreach (Room other in others)
            {
                if (Intersects(other))
                {
                    intersects = true;
                    break;
                }
            }
            if (!intersects) offset++;
            else offset = 0;
        }
    }
}

class ExitRoom : Room {
    /// <summary>
    /// Exit on the border
    /// </summary>
    /// <param name="most"></param>
    public ExitRoom(LevelGenerator.Boundaries most)
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                this.position = new Vector2Int(most.r + 3, Random.Range(most.b + 2, most.t - 2));
                most.r += 5;
                break;
            case 1:
                this.position = new Vector2Int(most.l - 3, Random.Range(most.b + 2, most.t - 2));
                most.l -= 5;
                break;
            case 2:
                this.position = new Vector2Int(Random.Range(most.l + 2, most.r - 2), most.t + 3);
                most.t += 5;
                break;
            case 3:
                this.position = new Vector2Int(Random.Range(most.l + 2, most.r - 2), most.b - 3);
                most.b -= 5;
                break;
            default:
                break;
        }
        this.roomType = RoomType.exit;
        this.moreThanOneConnections = false;
        this.height = 1;
        this.width = 1;
    }

    /// <summary>
    /// exit set by direction
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="others"></param>
    /// <param name="roomSpread"></param>
    public ExitRoom(Vector2 dir, List<Room> others,int roomSpread):base(Vector2Int.zero,1,1,RoomType.exit)
    {
        this.moreThanOneConnections = false;
        Deintersect(roomSpread, others, dir.normalized);
    }


    public override void GenerateRoomToMap(Vector2Int center, ref int[,] map)
    {
        position += center;
        map[position.x,position.y] = 3;
    }
}

class TreasureRoom : Room {
    public SecretRoom secret;
    public TreasureRoom(Vector2 dir, List<Room> others, int roomSpread,SecretRoom secret) : base(Vector2Int.zero, 1, 1, RoomType.treasure)
    {
        this.moreThanOneConnections = false;
        this.secret = secret;
        Deintersect(roomSpread, others, dir.normalized);
    }

    public override void GenerateRoomToMap(Vector2Int center, ref int[,] map)
    {
        position += center;
        map[position.x, position.y] = 4;
    }
}

class StartingRoom : Room
{
    public StartingRoom() : base(Vector2Int.zero, 3, 3, RoomType.start)
    {
        plan = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                plan[i, j] = 1;
            }

        }
    }
}