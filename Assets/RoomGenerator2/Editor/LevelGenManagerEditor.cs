using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenManager))]
public class LevelGenManagerEditor : Editor
{
    /*

    public override void OnInspectorGUI()
    {
        var genManager = (LevelGenManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate graph"))
        {
            genManager.init(null, false);
            int tries = genManager.LastGenerationTries();
            Debug.Log("Level generated after " + tries + " tries"); 
        }   
        
        GUILayout.Space(20);

        if (GUILayout.Button("Reset"))
        {
            genManager.reset();
        }

        GUILayout.Space(20);

        if(GUILayout.Button("Test Scene"))
        {
            float totalTries = 0;
            float maxTries = 0;
            float triesOver50 = 0;
            float triesOver100 = 0;
            float triesOver150 = 0;
            float triesOver200 = 0;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    genManager.StartCoroutine(genManager.init(null, false));
                    while (!genManager.SceneGenerated()) ;
                    int tries = genManager.LastGenerationTries();
                    totalTries += tries;
                    if(maxTries < tries) maxTries = tries;
                    if(tries > 50) triesOver50++;
                    if(tries > 100) triesOver100++;
                    if(tries > 150) triesOver150++;
                    if(tries > 200) triesOver200++;
                } catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
            float averageTries = totalTries/100f;
            Debug.Log("######## TEST COMPLETED ######## \nAverage number of tries: " 
            + averageTries + "\nMax tries to generate: " 
            + maxTries + "\n Tries over 50: " + triesOver50 +"%\nTries over 100: " + triesOver100 +"%\nTries over 150: " + triesOver150+"%\nTries over 200: " + triesOver200 +"%");
        }
    }
    */
}