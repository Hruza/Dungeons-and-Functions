using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MinimapDrawer : Graphic
{
    private List<MinimapTile> tiles;

    public struct MinimapTile {
        public Vector2Int pos;
        public Color color;
        public GeneratorV2.TileType type;
    } 
    private List<MinimapTile> Tiles {
        get {
            if (tiles == null)
            {
                tiles = new List<MinimapTile>();
            }
            return tiles;
        }
        set {
            if (tiles == null)
            {
                tiles = new List<MinimapTile>();
            }
            tiles = value;
        }
    }
    public int scale=5;

    public float timeIntervals = 0.1f;

    public int addingCount = 5;

    public Color vertexColor;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();
        UIVertex vertex = new UIVertex();
        
        foreach (MinimapTile tile in Tiles)
        {
            vertex.color = tile.color;
            vertex.position = scale * (Vector2)tile.pos;
            vh.AddVert(vertex);

            vertex.position = scale * ((Vector2)tile.pos + Vector2.up);
            vh.AddVert(vertex);

            vertex.position = scale * ((Vector2)tile.pos + Vector2.one );
            vh.AddVert(vertex);
            
            vertex.position = scale * ((Vector2)tile.pos + Vector2.right);
            vh.AddVert(vertex);
        }
        for (int i = 0; i < 4 * Tiles.Count; i += 4)
        {
            vh.AddTriangle(i+0, i+1, i+2);
            vh.AddTriangle(i+0, i+2, i+3);
        }   
    }

    public void AddRoom(List<MinimapTile> tiles, bool fast = false) {
        StartCoroutine(AddingRoom(tiles,fast));
    }

    private IEnumerator AddingRoom(List<MinimapTile> tiles,bool fast=false) {
        int i = 0;
        foreach (MinimapTile tile in tiles)
        {
            i++;
            this.Tiles.Add(tile);
            if (timeIntervals > 0)
            {
                if (tile.type == GeneratorV2.TileType.path || fast || i >= addingCount)
                {
                    this.SetVerticesDirty();
                    i = 0;
                    yield return new WaitForSeconds(timeIntervals);
                }
            }
        }
        this.SetVerticesDirty();
    }

    public void ChangeColor(Vector2Int pos,Color col) {
        tiles[tiles.FindIndex(x => x.pos == pos)]= new MinimapTile { color=col,pos=pos,type= tiles.Find(x => x.pos == pos).type };
    }
}
