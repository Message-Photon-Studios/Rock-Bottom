using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum RoomTypes
{
    STARTING,
    FINAL,
    NORMAL,
    ITEM
}

public enum RoomDiffs
{
    NO_ENCOUNTER,
    SUPER_EASY,
    EASY,
    NORMAL,
    HARD,
    SUPER_HARD
}
public class RoomData : MonoBehaviour
{
    public RoomTypes type;
    public RoomDiffs difficulty;

    public bool renderGizmos;
    
    [Space(10)]

    public bool leftDoor;
    public bool topDoor;
    public bool rightDoor;
    public bool bottomDoor;

    [HideInInspector]
    public int layoutCode
    {
        get
        {
            var _layoutCode = 0;
            if (leftDoor) _layoutCode += 8;
            if (bottomDoor) _layoutCode += 4;
            if (rightDoor) _layoutCode += 2;
            if (topDoor) _layoutCode += 1;
            return _layoutCode;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnDrawGizmos()
    {
        if (!renderGizmos) 
            return;
        Vector3 pos = transform.position;
        Gizmos.color = leftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(0, 20, 0));
        
        Gizmos.color = topDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 20, 0), pos + new Vector3(20, 20, 0));

        Gizmos.color = rightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 20, 0), pos + new Vector3(20, 0, 0));

        Gizmos.color = bottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(20, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
