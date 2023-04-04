using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    bool Update = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager targe = (GameManager)target;
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
