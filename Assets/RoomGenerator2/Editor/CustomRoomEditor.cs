using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomRoom))]
public class CustomRoomEditor : Editor
{
    public Direction newNodeDir, doorDir;
    public DoorColor doorColor;
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        var room = (CustomRoom)target;
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Room Editor", EditorStyles.boldLabel);
        room.selectedNode = EditorGUILayout.Vector2Field("Selected Node", room.selectedNode);

        EditorGUILayout.Space();

        if (GUILayout.Button("Delete selected node"))
        {
            room.deleteNode();
        }
        
        EditorGUILayout.Space();

        // Add field to select the direction to expand to
        newNodeDir = (Direction)EditorGUILayout.EnumPopup("Expansion Direction", newNodeDir);
        if (GUILayout.Button("Add node"))
        {
            room.addNode(newNodeDir);
        }
        
        EditorGUILayout.Space();

        doorColor = (DoorColor)EditorGUILayout.EnumPopup("Door color", doorColor);
        // Add field to select the direction in which to create a door
        doorDir = (Direction)EditorGUILayout.EnumPopup("Door direction", doorDir);
        if (GUILayout.Button("Toggle door"))
        {
            room.toggleDoor(doorDir, doorColor);
        }
    }
#endif
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnSceneGUI()
    {
        // when the mouse is clicked in the scene view get the mouse position and add a node there
        if (Event.current.type != EventType.MouseDown) 
            return;
        var mousePos = Event.current.mousePosition;
        var newVectorCoords = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        // Change selected node to the node in the coordinates
        var room = (CustomRoom)target;
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        // Divide the ray origin by 6 and round to the nearest integer
        var newCoords = new Vector2Int(Mathf.RoundToInt(ray.origin.x / 8), Mathf.RoundToInt(ray.origin.y / 8));
        room.selectedNode = newCoords;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
