using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom UI
/// </summary>
[CustomEditor(typeof(LevelGenerator))]
public class GeneratorEditor : Editor {

    Vector2Int param=Vector2Int.one;

    int[,] template;

    bool creating = false;

    List<GameObject> spawnedTiles;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LevelGenerator tgt = (LevelGenerator)target;

        if(!creating) param = (Vector2Int)EditorGUILayout.Vector2IntField("dimensions", param);


        if (GUILayout.Button("Prepare map"))
        {
            template = new int[param.x,param.y];
            creating = true;
        }

        if (creating)
        {
            EditorGUILayout.BeginVertical();
            for (int j = 0; j < param.y; j++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < param.x; i++)
                {
                    template[i, param.y - j - 1] = EditorGUILayout.IntField(template[i, param.y - j - 1]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Map"))
            {
                if (spawnedTiles != null)
                {
                    foreach (GameObject tile in spawnedTiles)
                    {
                        Destroy(tile);
                    }
                }
                spawnedTiles = new List<GameObject>();

                for (int i = 0; i < param.x; i++)
                {
                    for (int j = 0; j < param.y; j++)
                    {
                        spawnedTiles.Add(Instantiate(tgt.tile[template[i, j]], tgt.transform.position + new Vector3(i * 3, j * 3, 0), tgt.transform.rotation));
                    }
                }
            }

            if (GUILayout.Button("Stop"))
            {
                creating = false;
                foreach (GameObject tile in spawnedTiles)
                {
                    DestroyImmediate(tile);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Stop"))
            {

            }
        }
    }
}
