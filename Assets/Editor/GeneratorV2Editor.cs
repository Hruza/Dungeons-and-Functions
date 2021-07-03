using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom UI
/// </summary>
[CustomEditor(typeof(GeneratorV2))]
public class GeneratorV2Editor : Editor {

    Vector2Int param=Vector2Int.one;

    int[,] template;

    bool creating = false;

    List<GameObject> spawnedTiles;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GeneratorV2 tgt = (GeneratorV2)target;

        int seed = EditorGUILayout.IntField(-1);

        if (GUILayout.Button("Generate")) {
            tgt.Generate(tgt.debugLevel,seed);
        }
    }
}
