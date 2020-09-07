using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom UI
/// </summary>
[CustomEditor(typeof(RoomPrefab))]
public class RoomEditor : Editor {


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomPrefab roomPrefab = (RoomPrefab)target; 

        if (GUILayout.Button("Generate map"))
        {
            roomPrefab.Map = new RoomMap(roomPrefab);
        }

        if (GUILayout.Button("Test intersection"))
        {
            Debug.LogFormat("Intersects: {0}",roomPrefab.Intersects(roomPrefab.other));
        }
    }
}
