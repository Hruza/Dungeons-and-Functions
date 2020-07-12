using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorV2 : MonoBehaviour
{
    public static List<RoomPrefab> allRooms=null;
    public void Generate(Level level) {
        //load rooms
        


        //disperse rooms

        //create paths

        //create secrets
    }

    public GameObject[] PickRooms(int roomCount, int enemyCount, int advantageFactor, RoomPrefab.RoomType roomType= RoomPrefab.RoomType.combat) {
        
        //load rooms
        if (allRooms == null) {
            GameObject[] allRoomsObjects= Resources.LoadAll<GameObject>("Rooms");
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

        List<RoomPrefab> possible = allRooms.FindAll(x => x.roomType == roomType);

       int[] selected = new int[roomCount];

        //find fitting rooms
        //pick random
        for(int i =0;i<selected.Length;i++)
        {
            selected[i] = Random.Range(0, possible.Count);
        }

        //correcting
        Vector2 enemySpaceCounts;
        for (int i = 0; i < 40; i++)
        {
            enemySpaceCounts = CountEnemySpace(selected, possible);
            if (enemySpaceCounts.y < enemyCount) {

            }
            else if (enemySpaceCounts.x > enemyCount) {

            }
            else if (CountAdvantage(selected, possible) > advantageFactor) { 
            
            }
            else if (CountAdvantage(selected, possible) > advantageFactor)
            {

            }
        }

        return null;
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
}
