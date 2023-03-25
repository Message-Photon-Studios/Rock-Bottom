using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var roomManager = (RoomManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate graph"))
        {
            roomManager.init();
        }

        if (GUILayout.Button("Reset"))
        {
            roomManager.reset();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
