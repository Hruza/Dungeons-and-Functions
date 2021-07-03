using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private static Minimap _minimap;

    public MinimapDrawer drawer;

    public Color wallColor;
    public Color floorColor;
    public Color pathColor;
    public Color secretColor;
    public Color closedExitColor;
    public Color openExitColor;

    public static Minimap minimap
    {
        get
        {
            if (_minimap == null)
            {
                _minimap = (Minimap)FindObjectOfType(typeof(Minimap));
                if (_minimap == null)
                {
                    Debug.LogWarning("No minimap");
                }
            }

            return _minimap;
        }
    }

    Vector2Int lastTile;

    Transform player;

    RectTransform rt;

    bool exitOpen = false;

    void Update()
    {

        if (rt != null)
        {
            if (LevelController.exitOpen && !exitOpen) {
                exitOpen = true;
                if (discovered[GeneratorV2.exitPos.x, GeneratorV2.exitPos.y]) {
                    drawer.ChangeColor(GeneratorV2.exitPos, openExitColor);
                }
            }
            if (NewTile() && !discovered[lastTile.x, lastTile.y])
            {
                DiscoverNeighborhood(lastTile);
            }
            rt.localPosition = drawer.scale * ((-((Vector2)player.position - startPos) / GeneratorV2.gridSize) - (startMapPos));

            if (Input.GetKeyDown(KeyCode.M))
            {
                GetComponent<RectTransform>().sizeDelta = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta - (20 * Vector2.one);
                drawer.scale *= 2;
                drawer.SetVerticesDirty();
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                drawer.scale /= 2;
                GetComponent<RectTransform>().sizeDelta = Vector2.one * 150;
                drawer.SetVerticesDirty();
            }
        }
    }

    void DiscoverNeighborhood(Vector2Int pos)
    {
        List<MinimapDrawer.MinimapTile> toDraw = new List<MinimapDrawer.MinimapTile>();
        Queue<Vector2Int> Q = new Queue<Vector2Int>();
        Vector2Int current;
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Q.Enqueue(pos);
        GeneratorV2.TileType type = gen.Map[pos.x, pos.y].type;
        if (type == GeneratorV2.TileType.none || type == GeneratorV2.TileType.wall) {
            return;
        }
        while (Q.Count > 0)
        {
            current = Q.Dequeue();
            Color col=Color.magenta;
            GeneratorV2.TileType tileType=gen.Map[current.x, current.y].type;
            switch (tileType)
            {
                case GeneratorV2.TileType.room:
                    switch (gen.RoomTileAtPos(current))
                    {
                        // 0 Free
                        // 1 Wall
                        // 2 Floor
                        // 3 Door
                        case 1:
                            col = wallColor;
                            break;
                        case 2:
                            col = floorColor;
                            break;
                        case 3:
                            if (gen.Map[current.x, current.y].door.IsDoor)
                                col = pathColor;
                            else
                                col = wallColor;
                            break;
                        default:
                            break;
                    }
                    break;
                case GeneratorV2.TileType.path:
                case GeneratorV2.TileType.door:
                    col = pathColor;
                    break;
                case GeneratorV2.TileType.secret:
                    col = secretColor;
                    break;
                default:
                    break;
            }
            if (current == GeneratorV2.exitPos) {
                col = LevelController.exitOpen ? openExitColor : closedExitColor;
            }

            toDraw.Add(new MinimapDrawer.MinimapTile { pos=current,color=col,type=tileType});
            discovered[current.x, current.y] = true;
            try
            {
                foreach (Vector2Int vect in dirs)
                {
                    if (!discovered[current.x + vect.x, current.y + vect.y] && gen.Map[current.x + vect.x, current.y + vect.y].type == type
                        && !Q.Contains(current + vect)
                        && (type == GeneratorV2.TileType.room ? gen.Map[current.x + vect.x, current.y + vect.y].roomIndex == gen.Map[pos.x, pos.y].roomIndex : true))
                    {
                        Q.Enqueue(current + vect);
                    }
                }
            }
            catch (System.IndexOutOfRangeException) { 
            
            }
        }
        drawer.AddRoom(toDraw);
    }

    private bool NewTile() {
        if (gen.Real2Map(player.position)!=lastTile)
        {
            lastTile = gen.Real2Map(player.position);
            return true;
        }
        else return false;
    }

    private GeneratorV2 gen;

    private bool[,] discovered;

    Vector2 startPos;
    Vector2 startMapPos;

    public void Initialize(GeneratorV2 gen) {
        this.gen = gen;
        discovered = new bool[gen.mapWidth,gen.mapHeight];
        player = Player.player.transform;

        startPos = player.position;
        lastTile = gen.Real2Map(player.position);

        startMapPos = lastTile+(0.5f*Vector2.one);
        rt = drawer.GetComponent<RectTransform>();

        DiscoverNeighborhood(lastTile);
    }
}
