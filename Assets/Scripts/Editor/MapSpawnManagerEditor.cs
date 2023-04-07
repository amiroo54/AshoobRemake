using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapSpawnManager))]
public class MapSpawnManagerEditor : Editor
{
    bool Update = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapSpawnManager targe = (MapSpawnManager)target;
        Update = EditorGUILayout.Toggle("Generate Each Update (use with smaller res)", Update);
        if (GUILayout.Button("Generate"))
        {
            targe.SpawnMaps();
        }

        if (Update)
        {
            targe.SpawnMaps();
        }
        
    }
}
