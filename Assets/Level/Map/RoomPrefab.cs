using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

public class RoomPrefab : MonoBehaviour
{
    public enum RoomType { combat, exit, treasure, start };
    public RoomType roomType;
    public GameObject walls;
    public GameObject floors;
    public GameObject doors;

    public int minEnemies=3;
    public int maxEnemies=4;

    public int advantageFactor;
    
    public int cellSize = 4;
    public int Width {
        get {
            return Map.width;
        }
    }
    public int Height
    {
        get
        {
            return Map.height;
        }
    }

    public List<EnemyProperties> enemiesToSpawn {
        set {
            controller.EnemiesToSpawn = value; 
        }
        get {
            return controller.EnemiesToSpawn;
        }
    }

    public RoomController controller;

    [SerializeField]
    private RoomMap map;

    public RoomMap Map {
        get {
            if (map == null) {
                map = new RoomMap(this);
            }
            return map;
        }
        set {
            map = value;
        }
    }

    public List<GameObject> onEnter;
    public List<GameObject> onClear;

    public RoomPrefab other;
    public bool Intersects(RoomPrefab other)
    {
        return Map.Intersects(transform.position, other.Map, other.transform.position);
    }

    public void AlignWithGrid(Vector3 referencePoint, int gridSize) {
        Map.AlignWithGrid(referencePoint, gridSize,transform.position);
    }

}

public class RoomMap {

    // 0 Free
    // 1 Wall
    // 2 Floor
    // 3 Door
    public int[,] tileMap;

    public GameObject[,] tiles;

    private int cellSize;

    public int height;
    public int width;

    private Vector2 anchor;

    public RoomMap(RoomPrefab room) {
        cellSize = room.cellSize;
        SetBounds(room.walls.transform, room.cellSize,room.transform.position);

        tileMap = new int[width,height];
        tiles = new GameObject[width, height];

        FillMap(room.walls.transform, room.floors.transform,room.doors.transform,room.transform.position);
    }

    void FillMap(Transform walls,Transform floors,Transform doors,Vector2 roomPosition) {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tileMap[i, j] = 0;
            }
        }
        Vector2Int pos;
        foreach (Transform tr in walls)
        {
            pos = Real2Map(tr.position,roomPosition);
            tileMap[pos.x, pos.y] = 1;
            tiles[pos.x, pos.y] = tr.gameObject; 
        }
        foreach (Transform tr in floors)
        {
            pos = Real2Map(tr.position,roomPosition);
            tileMap[pos.x, pos.y] = 2;
            tiles[pos.x, pos.y] = tr.gameObject;
        }
        foreach (Transform tr in doors)
        {
            pos = Real2Map(tr.position, roomPosition);
            tileMap[pos.x, pos.y] = 3;
            tiles[pos.x, pos.y] = tr.gameObject;
        }
        Debug.Log(StringifyMap());
    }

    private string StringifyMap()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                    sb.Append(tileMap[j,i].ToString() + " ");
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

    private Vector2Int Real2Map(Vector2 pos,Vector2 roomPosition) {
        return new Vector2Int(Mathf.RoundToInt((pos.x-anchor.x-roomPosition.x)/cellSize), Mathf.RoundToInt((pos.y - anchor.y-roomPosition.y) / cellSize));
    }

    private Vector2 Map2Real(Vector2Int pos,Vector2 roomPosition)
    {
        return roomPosition+anchor+(cellSize*pos);
    }

    private void SetBounds(Transform walls,int cellSize,Vector3 pos) {
        float top;
        float bottom;
        float left;
        float right;

        if (walls.GetChild(0) == null)
        {
            Debug.LogError("MapError: there are no outer walls");
            return;
        }
        else
        {
            Transform child0 = walls.GetChild(0);
            top = child0.position.y;
            bottom = top;
            left = child0.position.x;
            right = left;
        }

        int i = 0;
        foreach (Transform tr in walls)
        {
            i++;
            top = Mathf.Max(top,tr.position.y);
            bottom = Mathf.Min(bottom, tr.position.y);
            right = Mathf.Max(right, tr.position.x);
            left = Mathf.Min(left, tr.position.x);
        }
        Debug.LogFormat("Found {0} walls",i);
        height =1+ Mathf.RoundToInt((top - bottom)/cellSize);
        width =1+ Mathf.RoundToInt((right - left) / cellSize);
        Debug.LogFormat("Room size is {0}x{1}", height,width);
        anchor = new Vector2(left-pos.x,bottom-pos.y);
    }

    public bool Intersects(Vector2 roomPos,RoomMap other,Vector2 otherRoomPos) {
        Vector2Int otherPos=new Vector2Int(0,0);
        Vector2Int localpos;
        for (int i = 0; i < other.width; i++)
        {
            otherPos.x = i;
            for (int j = 0; j < other.height; j++)
            {
                if (other.tileMap[i, j] > 0)
                {
                    otherPos.y = j;
                    localpos = Real2Map(other.Map2Real(otherPos, otherRoomPos), roomPos);
                    if (IsInRange(localpos)) {
                        if (tileMap[localpos.x,localpos.y]>0) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool IsInRange(Vector2Int pos) {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
        {
            return false;
        }
        else
            return true;
    }

    public void AlignWithGrid(Vector3 referencePoint, int gridSize,Vector2 roomPos)
    {
        Vector2 realAnchor = Map2Real(Vector2Int.zero, roomPos);
        Vector3 relativeAnchor = (Vector3)realAnchor - referencePoint;
        realAnchor.x = referencePoint.x + gridSize * Mathf.RoundToInt(relativeAnchor.x/gridSize);
        realAnchor.y = referencePoint.y + gridSize * Mathf.RoundToInt(relativeAnchor.y / gridSize);
        anchor = realAnchor - roomPos;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tileMap[i, j] > 0) {
                    tiles[i, j].transform.position = Map2Real(new Vector2Int(i,j),roomPos);
                }
            }
        }
    }

}
