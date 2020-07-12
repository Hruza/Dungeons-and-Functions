using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorV2 : MonoBehaviour
{
    const int gridSize = 4;

    public Level debugLevel;
    public bool debugMode=false;

    private void Start()
    {
        if (debugMode) {
            Generate(debugLevel);
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
    public void Generate(Level level) {
        //load rooms
        EnemyProperties[] enemies = EnemyBundle.Merge(level.enemies);
        List<RoomInfo> selectedRooms = PickRooms(level.roomCount,enemies.Length,level.advantageFactor);

        //instantiate and disperse rooms
        List<RoomInfo> createdRooms = new List<RoomInfo>();

        GameObject startingRoom = (GameObject)Instantiate(GetRoomOfType(RoomPrefab.RoomType.start).gameObject, transform.position, transform.rotation);
        createdRooms.Add(new RoomInfo { gameObject=startingRoom,info=startingRoom.GetComponent<RoomPrefab>()});

        foreach (RoomInfo rm in selectedRooms)
        {
            GameObject newRoomObject = Instantiate(rm.gameObject,transform.position,transform.rotation);
            RoomInfo newRoom = new RoomInfo { gameObject = newRoomObject, info = newRoomObject.GetComponent<RoomPrefab>() };
            Deintersect(newRoom,Random.insideUnitCircle.normalized*gridSize,createdRooms);
            createdRooms.Add(newRoom);
        }

        //distribute enemies
        

        //create paths

        //create secrets
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

        //fill over maximal capacity

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


    struct RoomInfo
    {
        public GameObject gameObject;
        public RoomPrefab info;
        public List<EnemyProperties> enemies;
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
                    if (enemyCount == enemySpaceCounts.x && selected[random] <= possible.Count - j)
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
