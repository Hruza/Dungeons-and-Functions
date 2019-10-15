using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomPattern
{
    public int[,] roomPlan;

    public int width;
    public int height;

    public int minEnemies;
    public int maxEnemies;


    public RoomPattern(int mapWidth, int mapHeigth, int minEnemies, int maxEnemies, int[,] roomPlan)
    {
        width = mapWidth;
        height = mapHeigth;
        this.minEnemies = minEnemies;
        this.maxEnemies = maxEnemies;
        this.roomPlan = roomPlan;
    }
}