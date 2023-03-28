using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Noise))]
public class NoiseEditor : Editor 
{           
    bool Update = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Noise targe = (Noise)target;
        Update = EditorGUILayout.Toggle("Generate Each Update (use with smaller res)", Update);
        if (GUILayout.Button("Generate"))
        {
            targe.UpdateMesh();
        }

        if (Update)
        {
            targe.UpdateMesh();
        }
        
    }
}
