using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR.WSA;
using System.Security.Principal;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Net.Http.Headers;
//using System;

public class GeneratorV2 : MonoBehaviour
{
    public const int gridSize = 4;

    public Level debugLevel;
    public bool debugMode=false;

    public GameObject pathTile;
    public GameObject wallTile;
    public GameObject blackTile;

    public float additionalPathsCoefficient = 1.2f;

    private void Start()
    {
        if (debugMode) {
            float time = Time.realtimeSinceStartup;
            Generate(debugLevel);
            Debug.Log(Time.realtimeSinceStartup - time);
        }
    }

    private static List<RoomPrefab> allRooms=null;

    public static List<RoomPrefab> AllRooms {
        get {
            //load rooms
            if (allRooms == null)
            {
                GameObject[] allRoomsObjects = Resources.LoadAll<GameObject>("Rooms");
                allRooms = new List<RoomPrefab>();
                foreach (GameObject roomObject in allRoomsObjects)
                {
                    allRooms.Add(roomObject.GetComponent<RoomPrefab>());
                }
                allRooms.Sort(delegate (RoomPrefab x, RoomPrefab y)
                {
                    if (x.minEnemies == y.minEnemies) return x.maxEnemies.CompareTo(y.maxEnemies);
                    else return x.minEnemies.CompareTo(y.maxEnemies);
                });
            }
            return allRooms;
        }
    }

    private List<RoomInfo> roomList;

    public enum GeneratorPreset { normal,boss };

    public void Generate(Level level) {
        //load rooms
        EnemyProperties[] enemies = EnemyBundle.Merge(level.enemies);
        List<RoomInfo> selectedRooms = PickRooms(level.roomCount,enemies.Length,level.advantageFactor);

        //instantiate and disperse rooms
        List<RoomInfo> createdRooms = new List<RoomInfo>();

        //create starting rooms
        GameObject startingRoom = (GameObject)Instantiate(GetRoomOfType(RoomPrefab.RoomType.start).gameObject, transform.position, transform.rotation,transform);
        createdRooms.Add(new RoomInfo { gameObject=startingRoom,info=startingRoom.GetComponent<RoomPrefab>()});

        //create combat rooms
        GameObject newRoomObject;
        RoomInfo newRoom;
        foreach (RoomInfo rm in selectedRooms)
        {
            newRoomObject = Instantiate(rm.gameObject,transform.position,transform.rotation * Quaternion.Euler(0, 0, 90f * Random.Range(0, 4)), transform);
            newRoom = new RoomInfo { gameObject = newRoomObject, info = newRoomObject.GetComponent<RoomPrefab>() };
            Deintersect(newRoom,Random.insideUnitCircle.normalized*gridSize,createdRooms);
            createdRooms.Add(newRoom);
        }

        if (level.bossRoom) {
            RoomInfo rm = GetRoomOfType(RoomPrefab.RoomType.boss);
            newRoomObject = Instantiate(rm.gameObject, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90f * Random.Range(0, 4)), transform);
            newRoom = new RoomInfo { gameObject = newRoomObject, info = newRoomObject.GetComponent<RoomPrefab>() };
            Vector2 dir = Quaternion.Euler(0, 0, 90f * Random.Range(0, 4)) * Vector2.up * gridSize;
            Deintersect(newRoom,dir, createdRooms);
            createdRooms.Add(newRoom);
            
            //create exit
            newRoomObject = (GameObject)Instantiate(GetRoomOfType(RoomPrefab.RoomType.exit).gameObject, newRoom.gameObject.transform.position, transform.rotation, transform);
            newRoom = new RoomInfo { gameObject = newRoomObject, info = newRoomObject.GetComponent<RoomPrefab>() };
            Deintersect(newRoom, dir, createdRooms);
            createdRooms.Add(newRoom);
        }
        else {
            //create exit
            newRoomObject = (GameObject)Instantiate(GetRoomOfType(RoomPrefab.RoomType.exit).gameObject, transform.position, transform.rotation, transform);
            newRoom = new RoomInfo { gameObject = newRoomObject, info = newRoomObject.GetComponent<RoomPrefab>() };
            Deintersect(newRoom, Random.insideUnitCircle.normalized * gridSize, createdRooms);
            createdRooms.Add(newRoom);
        }

        //distribute enemies
        DistributeEnemies(enemies, createdRooms);

        //create map
        CreateMap(createdRooms);
        Debug.Log(StringifyMap());

        //create paths
        ConnectRooms(createdRooms,level.bossRoom);

        //create secrets
        foreach (SecretRoom secret in level.secretRooms)
        {
            CreateSecret(secret);
        }

        CreateBlackOutline();

        roomList = createdRooms;

        Minimap.minimap.Initialize(this);
    }

    private bool IsAdjacent(int x, int y, TileType type = TileType.none) {
        if (IsInRange(x, y) &&  map[x , y ].type==TileType.none) { 
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i != 0 || j != 0) && IsInRange(x + i, y + j) && ((type == TileType.none) ? map[x + i, y + j].type != TileType.none : map[x + i, y + j].type == type))
                    {
                        return true;
                    }
                }
            } 
        }
        return false;
    }

    private void CreateBlackOutline() {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsAdjacent(x,y)) {
                    PlaceTile(blackTile, new Vector2Int(x, y),TileType.none);
                }
            }
        }
    }

    private void CreateSecret(SecretRoom secret) { 
        
    }

    private int ManhattanMetric(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x-b.x)+ Mathf.Abs(a.y-b.y);
    }

    private bool DoorNotObstructed(Vector2Int a) {
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down,Vector2Int.left,Vector2Int.right }  ;
        Vector2Int point;
        foreach (Vector2Int vect in dirs)
        {
            point = a + vect;
            if (point.x==Mathf.Clamp(point.x,0,mapWidth-1) && point.y == Mathf.Clamp(point.y, 0, mapHeight - 1) && (map[point.x,point.y].type!=TileType.room) )
            {
                return true;
            }
        }
        return false;
    }

    private void ConnectRooms(List<RoomInfo> roomList,bool bossRoomIncluded) {
        bool[,] connectionMatrix = new bool[roomList.Count, roomList.Count];
        Debug.Log("connecting started");
        List<int> connected = new List<int> { 0 };
        while (connected.Count < roomList.Count)
        {
            List<Path> paths=new List<Path>();
            foreach (int i in connected.FindAll(x => roomList[x].doors.Count > 0))
            {                                               //search each connected room with free doors
                foreach (Vector2Int door1 in roomList[i].doors.FindAll(x => DoorNotObstructed(x)))
                {
                    paths.AddRange(Path.FindAllPaths(door1, map, mapWidth, mapHeight, connected));
                }
            }
            if (paths.Count == 0) {
                Debug.LogError("Cannot connect rooms");
                break;
            }

            CreateShortestPath(paths, ref roomList, ref connectionMatrix, ref connected,bossRoomIncluded);
        }
        for (int j = 0; j < Mathf.RoundToInt((additionalPathsCoefficient * roomList.Count)-roomList.Count ); j++)
        {
            List<Path> paths = new List<Path>();
            for (int i = 0; i < roomList.Count; i++)
            {                                               
                connected = new List<int> { i };
                for (int x = 0; x < roomList.Count; x++)
                {
                    if (connectionMatrix[i, x])
                    {
                        connected.Add(x);
                    }
                }
                foreach (Vector2Int door1 in roomList[i].doors.FindAll(x => DoorNotObstructed(x)))
                {
                    paths.AddRange(Path.FindAllPaths(door1, map, mapWidth, mapHeight,connected));
                }
            }
            if (paths.Count > 0)
            {
                CreateShortestPath(paths, ref roomList, ref connectionMatrix,ref connected,bossRoomIncluded);
            }
        }
        DeleteAllDoorsLeft();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsAdjacent(x, y,TileType.path))
                {
                    PlaceTile(wallTile, new Vector2Int(x, y), TileType.wall);
                }
            }
        }
    }

    private bool IsOK(Path path,int count) {
        return (map[path.to.x, path.to.y].roomIndex < count - 2 && map[path.from.x, path.from.y].roomIndex < count - 2)
            || (map[path.to.x, path.to.y].roomIndex + map[path.from.x, path.from.y].roomIndex) == (2 * count) - 3;
    }

    private void CreateShortestPath(List<Path> paths,ref List<RoomInfo> roomList, ref bool[,] connectionMatrix, ref List<int> connected,bool bossRoomIncluded) {
        int min = paths[0].Length;
        Path minPath = paths[0];
        foreach (Path path in paths)
        {
            if (path.Length < min && (!bossRoomIncluded || IsOK(path,roomList.Count) ))
            {
                minPath = path;
                min = path.Length;
            }
        }

        minPath.CreatePath(this, pathTile, wallTile);

        int toRoom = map[minPath.to.x, minPath.to.y].roomIndex;
        int fromRoom = map[minPath.from.x, minPath.from.y].roomIndex;
        if(connected!=null)connected.Add(toRoom);
        connectionMatrix[fromRoom, toRoom] = true;
        connectionMatrix[toRoom, fromRoom] = true;

        MarkDoorUsed(minPath.to, roomList);
        MarkDoorUsed(minPath.from, roomList);

        if (roomList[toRoom].info.roomType == RoomPrefab.RoomType.exit)
        {
            List<Vector2Int> exitDoors = new List<Vector2Int>(roomList[toRoom].doors);
            foreach (Vector2Int pos in exitDoors)
            {
                map[pos.x, pos.y].door.IsDoor = false;
                MarkDoorUsed(pos, roomList);
            }
        }
    }

    private void MarkDoorUsed(Vector2Int pos, List<RoomInfo> roomList) {
        int roomIndex = map[pos.x, pos.y].roomIndex;
        map[pos.x, pos.y].type = TileType.room;
        roomList[roomIndex].doors.Remove(pos);
    }


    private void DeleteAllDoorsLeft()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (map[i, j].type == TileType.door) {
                    map[i, j].door.IsDoor = false;
                    map[i, j].type = TileType.room;
                }
            }
        }
    }

    private bool IsInRange(int x, int y ) {
        return x == Mathf.Clamp(x, 0, mapWidth - 1) && y == Mathf.Clamp(y, 0, mapHeight - 1);
    }

    private bool IsInRange(Vector2Int pos)
    {
        return pos.x == Mathf.Clamp(pos.x, 0, mapWidth - 1) && pos.y == Mathf.Clamp(pos.y, 0, mapHeight - 1);
    }

    public void PlaceTile(GameObject gameObject, Vector2Int pos, TileType type) {
        if (IsInRange(pos)) {
            GameObject tile = (GameObject)Instantiate(gameObject, Map2Real(pos), transform.rotation, transform);
            tile.transform.localScale = new Vector3(gridSize,gridSize,1);
            map[pos.x, pos.y].type = type;
            map[pos.x, pos.y].tile = tile;
        }
    }

    public void PlaceObject(GameObject gameObject, Vector2Int pos)
    {
        if (IsInRange(pos))
        {
            Instantiate(gameObject, Map2Real(pos), transform.rotation, transform);
        }
    }

    private class Path {
        const int maxSearch = 20;
        private List<Vector2Int> pathTiles;
        public Vector2Int from;
        public Vector2Int to;
        public Path(Vector2Int from, Vector2Int to) { 

        }

        private Path(List<Vector2Int> pathTiles) {
            this.from = pathTiles[0];
            this.to = pathTiles[pathTiles.Count-1];
            this.pathTiles = pathTiles;
        }

        public void DrawPath(GeneratorV2 gen,Color col,float duration=1) {
            for(int i =0; i<pathTiles.Count-1;i++)
            {
                Debug.DrawLine(gen.Map2Real(pathTiles[i]),gen.Map2Real(pathTiles[i+1]),col,duration);
            }
        }

        public void CreatePath(GeneratorV2 gen,GameObject pathTile,GameObject wallTile)
        {
            for (int i = 1; i < pathTiles.Count - 1; i++)
            {
                gen.PlaceTile(pathTile, pathTiles[i],TileType.path);
            }
        }

        static private int GetValue(int[,] map,Vector2Int pos) {
            try
            {

                return map[pos.x, pos.y];
            }
            catch (System.IndexOutOfRangeException)
            {

                return -3;
            }
        }

        static private Path TraceBack(Vector2Int from, Vector2Int to, int[,] pathMap) {
            List<Vector2Int> tiles = new List<Vector2Int>();
            tiles.Add(from);
            Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2Int point;
            Vector2Int currentTile = from;
            for (int i = GetValue(pathMap,from); i > 0; i--)
            {
                foreach (Vector2Int vect in dirs)
                {
                    point = currentTile + vect;
                    if ( GetValue(pathMap,point) == i-1) {
                        tiles.Add(point);
                        currentTile = point;
                        break;
                    }
                }
            }
            tiles.Reverse();
            return new Path(tiles);
        }

        static public List<Path> FindAllPaths(Vector2Int from, MapTile[,] map,int width,int height, List<int> ignoreIndices) {
            int[,] pathMap = new int[width,height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    switch (map[i,j].type)
                    {
                        case TileType.room:
                            pathMap[i, j]=-3;
                            break;
                        case TileType.door:
                            if (ignoreIndices.Contains(map[i, j].roomIndex))
                            {
                                pathMap[i, j] = -3;
                            }
                            else {
                                pathMap[i, j] = -2;
                            }
                            break;
                        default:
                            pathMap[i, j] = -1;
                            break;
                    }
                }
            }
            pathMap[from.x, from.y] = 0;

            List<Path> paths = new List<Path>();
            Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            for (int radius = 1; radius < 30; radius++)
            {
                for (int x = Mathf.Max(0,from.x-radius); x < Mathf.Min(width,from.x + radius); x++)
                {
                    for (int y = Mathf.Max(0,from.y - radius); y < Mathf.Min(height,from.y + radius); y++)
                    {
                        if (pathMap[x,y]==radius-1) {
                            foreach (Vector2Int vect in dirs)
                            {
                                if ( x + vect.x == Mathf.Clamp(x + vect.x,0,width-1) && y + vect.y == Mathf.Clamp(y + vect.y, 0, height-1)) {
                                    if (pathMap[x + vect.x, y + vect.y] == -1)
                                    {
                                        pathMap[x + vect.x, y + vect.y] = radius;
                                    }
                                    if (pathMap[x + vect.x, y + vect.y] == -2)
                                    {
                                        pathMap[x + vect.x, y + vect.y] = radius;
                                        paths.Add(TraceBack(new Vector2Int(x + vect.x, y + vect.y), from, pathMap));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return paths;
        }

        public int Length {
            get {
                return pathTiles.Count;
            }
        }
    }

    public enum TileType { none , room , path ,wall , door }
    public struct MapTile {
        public TileType type;
        public int roomIndex;
        public RoomDoor door;
        public GameObject tile;
    }

    public MapTile[,] Map {
        get {
            return map;
        }
    }
    private MapTile[,] map;
    private Vector2 anchor;
    public int mapWidth;
    public int mapHeight;

    private void CreateMap(List<RoomInfo> rooms) {

        Vector4 boundaries = GetBoundaries(rooms);
        mapWidth = Mathf.RoundToInt((boundaries.z - boundaries.x) / gridSize);
        mapHeight = Mathf.RoundToInt((boundaries.w - boundaries.y) / gridSize);
        map = new MapTile[mapWidth,mapHeight];
        anchor = Gridify(new Vector2( boundaries.x , boundaries.y ))-(Vector2)transform.position;

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] = new MapTile { type = TileType.none, roomIndex = -1, door = null };
            }
        }

        for(int i = 0; i < rooms.Count;i++)
        {
            RoomToMap(rooms[i], i);
        }
    }

    void RoomToMap(RoomInfo room,int roomIndex) {
        Vector2Int roomLB = Real2Map(room.info.Map.Map2Real(Vector2Int.zero,room.gameObject.transform.position));
        room.doors = new List<Vector2Int>();
        for (int i = 0; i < room.info.Map.width; i++)
        {
            for (int j = 0; j < room.info.Map.height; j++)
            {
                try
                {
                    int tile = room.info.Map.tileMap[i, j];
                    if (tile == 3)
                    {
                        map[roomLB.x + i, roomLB.y + j].door = room.info.GetDoorAtPosition(i, j);
                        map[roomLB.x + i, roomLB.y + j].roomIndex = roomIndex;
                        map[roomLB.x + i, roomLB.y + j].type = TileType.door;
                        room.doors.Add(new Vector2Int(roomLB.x + i, roomLB.y + j));
                    }
                    else if (tile > 0)
                    {
                        map[roomLB.x + i, roomLB.y + j].roomIndex = roomIndex;
                        map[roomLB.x + i, roomLB.y + j].type = TileType.room;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogErrorFormat("LB: {0}, {1}; RT: {2}, {3}", Map2Real(Vector2Int.zero).x, Map2Real(Vector2Int.zero).y, Map2Real(new Vector2Int(mapWidth-1,mapHeight-1)).x, Map2Real(new Vector2Int(mapWidth - 1, mapHeight - 1)).y);
                    Debug.LogErrorFormat("i: {0}, j: {1} \n room.x: {2}, room.y:{3} \n mapWidth: {4}, mapHeight: {5}",i,j, roomLB.x, roomLB.y, mapWidth,mapHeight);
                    throw;
                }

            }
        }
    }

    public Vector2Int Real2Map(Vector2 pos)
    {
        return new Vector2Int(Mathf.RoundToInt((pos.x - anchor.x - transform.position.x) / gridSize), Mathf.RoundToInt((pos.y - anchor.y - transform.position.y) / gridSize));
    }

    private string StringifyMap()
    {
        StringBuilder sb = new StringBuilder();

        for (int j = mapHeight - 1; j >= 0;j--)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                sb.Append(map[i, j].roomIndex.ToString() + " ");
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

    public RoomPrefab GetRoomAtPos(Vector2Int pos) {
        int index = Map[pos.x, pos.y].roomIndex;
        if (index >= 0) {
            return roomList[index].info;
        }
        return null;
    }

    public int RoomTileAtPos(Vector2Int pos)
    {
        int index = Map[pos.x, pos.y].roomIndex;
        if (index >= 0)
        {
            RoomInfo rm = roomList[index];
            Vector2Int roomPos = rm.info.Map.Real2Map(Map2Real(pos), rm.gameObject.transform.position);
            return rm.info.Map.tileMap[roomPos.x, roomPos.y];
        }
        return -1;
    }

    public Vector2 Map2Real(Vector2Int pos)
    {
        return (Vector2)transform.position + anchor + (gridSize * pos);
    }

    private Vector4 GetBoundaries(List<RoomInfo> rooms) {
        float left=rooms[1].gameObject.transform.position.x- (gridSize*rooms[1].info.Width);
        float bottom = rooms[1].gameObject.transform.position.y + (gridSize * rooms[1].info.Height); ;
        float right = rooms[1].gameObject.transform.position.x - (gridSize * rooms[1].info.Width); ;
        float top = rooms[1].gameObject.transform.position.y + (gridSize * rooms[1].info.Height); ;

        int i = 0;
        foreach (RoomInfo rm in rooms)
        {
            i++;
            left = Mathf.Min(left, rm.gameObject.transform.position.x - (gridSize * rm.info.Width));
            bottom = Mathf.Min(bottom, rm.gameObject.transform.position.y - (gridSize * rm.info.Width));
            right = Mathf.Max(right, rm.gameObject.transform.position.x + (gridSize * rm.info.Width));
            top = Mathf.Max(top, rm.gameObject.transform.position.y + (gridSize * rm.info.Width));
        }
        return new Vector4(left,bottom,right,top);
    }

    public Vector2 Gridify(Vector2 pos) {
        Vector2 relativePos = (pos - (Vector2)transform.position)/gridSize;
        relativePos.x = Mathf.Round(relativePos.x);
        relativePos.y = Mathf.Round(relativePos.y);
        return (Vector2)transform.position + (relativePos * gridSize);
    }

    private void DistributeEnemies(EnemyProperties[] enemies,List<RoomInfo> rooms) {
        int i = 0;

        //fill minimal capacity
        foreach (RoomInfo room in rooms)
        {
            if (room.info.roomType == RoomPrefab.RoomType.combat)
            {
                List<EnemyProperties> basicEnemies = new List<EnemyProperties>();
                while (basicEnemies.Count < room.info.minEnemies && i < enemies.Length) {
                    basicEnemies.Add(enemies[i]);
                    i++;
                }
                room.info.enemiesToSpawn = basicEnemies;
            }
        }

        //fill to maximal capacity
        bool added = true;
        while (i < enemies.Length && added) {
            added = false;
            foreach (RoomInfo room in rooms)
            {
                if (room.info.roomType == RoomPrefab.RoomType.combat && room.info.enemiesToSpawn.Count < room.info.maxEnemies && i < enemies.Length)
                {
                    added = true;
                    room.info.enemiesToSpawn.Add(enemies[i]);
                    i++;
                }
            }
        }

        //fill over maximal capacity
        while (i < enemies.Length && added)
        {
            added = false;
            foreach (RoomInfo room in rooms)
            {
                if (room.info.roomType == RoomPrefab.RoomType.combat && i < enemies.Length)
                {
                    added = true;
                    room.info.enemiesToSpawn.Add(enemies[i]);
                    i++;
                }
            }
        }
    }

    private RoomInfo GetRoomOfType(RoomPrefab.RoomType type) {
        List<RoomPrefab> possible = AllRooms.FindAll(x => x.roomType == type);
        int randomIndex = Random.Range(0, possible.Count);
        return new RoomInfo { info=possible[randomIndex],gameObject= possible[randomIndex].gameObject };
    }

    private List<RoomInfo> GetRoomOfType(RoomPrefab.RoomType type,int count)
    {
        List<RoomPrefab> possible = AllRooms.FindAll(x => x.roomType == type);
        int randomIndex;
        List<RoomInfo> result = new List<RoomInfo>();
        for (int i = 0; i < count; i++)
        {
            randomIndex = Random.Range(0, possible.Count);
            result.Add(new RoomInfo { info = possible[randomIndex], gameObject = possible[randomIndex].gameObject });
        }
        return result;
    }


    class RoomInfo
    {
        public GameObject gameObject;
        public RoomPrefab info;
        public List<Vector2Int> doors;
    }

    private List<RoomInfo> PickRooms(int roomCount, int enemyCount, int advantageFactor, RoomPrefab.RoomType roomType= RoomPrefab.RoomType.combat) {
        


        List<RoomPrefab> possible = AllRooms.FindAll(x => x.roomType == roomType);

       int[] selected = new int[roomCount];

        //find fitting rooms
        //pick random
        for(int i =0;i<selected.Length;i++)
        {
            selected[i] = Random.Range(0, possible.Count);
        }

        //correcting
        Vector2 enemySpaceCounts;
        int random;
        for (int i = 0; i < 100; i++)
        {
            enemySpaceCounts = CountEnemySpace(selected, possible);
            random = Random.Range(0,roomCount);
            if (enemySpaceCounts.y < enemyCount)
            {
                if (selected[random] < possible.Count - 1)
                {
                    selected[random]++;
                }
            }
            else if (enemySpaceCounts.x > enemyCount)
            {
                if (selected[random] > 0)
                {
                    selected[random]--;
                }
            }
            else if ((CountAdvantage(selected, possible) - advantageFactor)!=0)
            {
                int diff = advantageFactor - CountAdvantage(selected, possible);
                int advantage = possible[selected[random]].advantageFactor;
                for (int j = 1; j < 3; j++)
                {
                    if (enemyCount > enemySpaceCounts.x && selected[random] >= j)
                    {
                        if (Mathf.Abs(diff) < Mathf.Abs(diff - (advantage - possible[selected[random] - j].advantageFactor)))
                        {
                            selected[random] = selected[random] - j;
                        }
                    }
                    if (enemyCount == enemySpaceCounts.x && selected[random] < possible.Count - j)
                    {
                        if (Mathf.Abs(diff) < Mathf.Abs(diff - (advantage - possible[selected[random] + j].advantageFactor)))
                        {
                            selected[random] = selected[random] + j;
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }

        List<RoomInfo> result = new List<RoomInfo>();

        for (int i = 0; i < selected.Length; i++)
        {
            result.Add(new RoomInfo
            {
                info = possible[selected[i]],
                gameObject = possible[selected[i]].gameObject
            }
            );
        }

        return result;
    }

    private Vector2 CountEnemySpace(int[] selected,List<RoomPrefab> rooms) {
        Vector2 counts = Vector2.zero;
        foreach (int index in selected)
        {
            counts.x += rooms[index].minEnemies;
            counts.y += rooms[index].maxEnemies;
        }
        return counts;
    }

    private int CountAdvantage(int[] selected, List<RoomPrefab> rooms) {
        int balance = 0;
        foreach (int index in selected)
        {
            balance += rooms[index].advantageFactor;
        }
        return balance;
    }

    private void Deintersect(RoomInfo room, Vector2 dir,List<RoomInfo> others,int additionalIterations =1) {
        int countdown = 1 + additionalIterations;
        bool intersects = false;
        while (countdown>0)
        {
            room.gameObject.transform.position += (Vector3)dir; 
            intersects = false;
            foreach (RoomInfo other in others)
            {
                if (room.info.Intersects(other.info)) {
                    intersects = true;
                    break;
                }
            }
            if (!intersects) {
                countdown--;
            }
        }
        room.info.AlignWithGrid(transform.position,gridSize);
    }
}
