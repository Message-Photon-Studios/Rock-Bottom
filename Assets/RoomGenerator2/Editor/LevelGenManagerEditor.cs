using System;
using UnityEditor;
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
            genManager.init(null, false);
        }
        
        GUILayout.Space(20);

        if (GUILayout.Button("Reset"))
        {
            genManager.reset();
        }

        GUILayout.Space(20);

        if(GUILayout.Button("Fast test"))
        {
            float failed = 0;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    genManager.init(null, false);
                } catch (Exception e)
                {
                    Debug.Log(e);
                    failed++;
                }
            }

            Debug.Log("######## TEST COMPLETED ######## Success chance: " + (100f-failed) + "%, total tests made: 100");
        }

        if(GUILayout.Button("Big test (SLOW!)"))
        {
            float failed = 0;
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    genManager.init(null, false);
                } catch (Exception e)
                {
                    failed++;
                }
            }

            Debug.Log("######## TEST COMPLETED ######## Success chance: " + (1000f-failed)/10f + "%, total tests made: 1000");
        }
    }
    
}