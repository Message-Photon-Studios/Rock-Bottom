﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenManager))]
public class LevelGenManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var genManager = (LevelGenManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate graph"))
        {
            genManager.init();
        }
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Step"))
        {
            genManager.step();
        }

        if (GUILayout.Button("Reset"))
        {
            genManager.reset();
        }
    }
}