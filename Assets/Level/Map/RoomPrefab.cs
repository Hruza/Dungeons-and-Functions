using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoomPrefab : MonoBehaviour
{
    public enum RoomType { combat, exit, treasure, start, boss, unused };
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
        Map.AlignWithGrid(referencePoint, gridSize,transform);
    }

    public RoomDoor GetDoorAtPosition(int x, int y)
    {
        return map.GetDoorAtPosition(x, y, transform.position);
    }
    public RoomDoor GetDoorAtWordPos(Vector2 pos)
    {
        return map.GetDoorAtWorldPos(pos, transform.position);
    }

    public Texture2D CreateTexture( Color floor, Color wall, Color none) {
        Texture2D texture = new Texture2D(Height, Width);
        return map.FillTexture(texture,floor,wall,none);
    }
}

public class RoomMap {

    // 0 Free
    // 1 Wall
    // 2 Floor
    // 3 Door
    public int[,] tileMap;

    private List<RoomDoor> roomDoors;

    private int cellSize;

    public int height;
    public int width;

    private Vector2 anchor;

    public RoomMap(RoomPrefab room) {
        cellSize = room.cellSize;
        SetBounds(room.walls.transform, room.cellSize, room.transform.position);

        tileMap = new int[width, height];

        FillMap(room.walls.transform, room.floors.transform, room.doors.transform, room.transform.position);
    }

    void FillMap(Transform walls, Transform floors, Transform doors, Vector2 roomPosition) {
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
            pos = Real2Map(tr.position, roomPosition);
            tileMap[pos.x, pos.y] = 1;
        }
        foreach (Transform tr in floors)
        {
            pos = Real2Map(tr.position, roomPosition);
            tileMap[pos.x, pos.y] = 2;
        }
        roomDoors = new List<RoomDoor>();
        foreach (Transform tr in doors)
        {
            pos = Real2Map(tr.position, roomPosition);
            tileMap[pos.x, pos.y] = 3;
            roomDoors.Add(tr.GetComponent<RoomDoor>());
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
                sb.Append(tileMap[j, i].ToString() + " ");
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

    public Vector2Int Real2Map(Vector2 pos, Vector2 roomPosition) {
        return new Vector2Int(Mathf.RoundToInt((pos.x - anchor.x - roomPosition.x) / cellSize), Mathf.RoundToInt((pos.y - anchor.y - roomPosition.y) / cellSize));
    }

    public Vector2 Map2Real(Vector2Int pos, Vector2 roomPosition)
    {
        return roomPosition + anchor + (cellSize * pos);
    }

    private void SetBounds(Transform walls, int cellSize, Vector3 pos) {
        float top;
        float bottom;
        float left;
        float right;

        if (walls.childCount == 0)
        {
            Debug.LogWarning("MapError: there are no outer walls");
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
            top = Mathf.Max(top, tr.position.y);
            bottom = Mathf.Min(bottom, tr.position.y);
            right = Mathf.Max(right, tr.position.x);
            left = Mathf.Min(left, tr.position.x);
        }
        Debug.LogFormat("Found {0} walls", i);
        height = 1 + Mathf.RoundToInt((top - bottom) / cellSize);
        width = 1 + Mathf.RoundToInt((right - left) / cellSize);
        Debug.LogFormat("Room size is {0}x{1}", height, width);
        anchor = new Vector2(left - pos.x, bottom - pos.y);
    }

    public bool Intersects(Vector2 roomPos, RoomMap other, Vector2 otherRoomPos) {
        Vector2Int otherPos = new Vector2Int(0, 0);
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
                        if (tileMap[localpos.x, localpos.y] > 0) {
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

    public void AlignWithGrid(Vector3 referencePoint, int gridSize, Transform roomTransform)
    {
        Vector2 realAnchor = Map2Real(Vector2Int.zero, roomTransform.position);
        Vector3 relativeAnchor = (Vector3)realAnchor - referencePoint;
        realAnchor.x = referencePoint.x + gridSize * Mathf.RoundToInt(relativeAnchor.x / gridSize);
        realAnchor.y = referencePoint.y + gridSize * Mathf.RoundToInt(relativeAnchor.y / gridSize);
        roomTransform.position = realAnchor - anchor;
    }

    public RoomDoor GetDoorAtPosition(int x, int y, Vector2 roomPos) {
        if (tileMap[x, y] == 3)
        {
            return roomDoors.Find(rm => Real2Map(rm.transform.position, roomPos) == new Vector2Int(x, y));
        }
        else
            return null;
    }
    public RoomDoor GetDoorAtWorldPos(Vector2 position, Vector2 roomPos)
    {
        Vector2Int mapPos = Real2Map(position, roomPos);
        if (IsInRange(mapPos) && tileMap[mapPos.x, mapPos.y] == 3)
        {
            return roomDoors.Find(x => Real2Map(x.transform.position, roomPos) == mapPos);
        }
        else
            return null;
    }

    public Texture2D FillTexture(Texture2D texture,Color floor, Color wall, Color none)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color;
                switch (tileMap[x,y])
                {
                    case 0:
                        color = none;
                        break;
                    case 1:
                        color = wall;
                        break;
                    case 2:
                    default:
                        color = floor;
                        break;
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}
