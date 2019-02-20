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

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator myScript = (LevelGenerator)target;
       

        param = (Vector2Int)EditorGUILayout.Vector2IntField("Map dimensions", param);
        if (GUILayout.Button("Build Object"))
        {
            //myScript.Generate(param.y,param.y);
        }
    }
}
