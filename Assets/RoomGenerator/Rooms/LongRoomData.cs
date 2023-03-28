using System.Collections.Generic;
using UnityEngine;

public class LongRoomData : MonoBehaviour
{
    public bool leftDoor;
    public bool rightDoor;
    public bool topLeftDoor;
    public bool topRightDoor;
    public bool bottomLeftDoor;
    public bool bottomRightDoor;

    [Space(10)]
    public bool renderGizmos;
    
    [HideInInspector]
    public int layoutCode
    {
        get
        {
            var _layoutCode = 0;
            if (leftDoor) _layoutCode += 32;
            if (rightDoor) _layoutCode += 16;
            if (topLeftDoor) _layoutCode += 8;
            if (topRightDoor) _layoutCode += 4;
            if (bottomLeftDoor) _layoutCode += 2;
            if (bottomRightDoor) _layoutCode += 1;
            return _layoutCode;
        }
    }
    
    [HideInInspector]
    public int mirroredLayoutCode
    {
        get
        {
            var _layoutCode = 0;
            if (rightDoor) _layoutCode += 32;
            if (leftDoor) _layoutCode += 16;
            if (topRightDoor) _layoutCode += 8;
            if (topLeftDoor) _layoutCode += 4;
            if (bottomRightDoor) _layoutCode += 2;
            if (bottomLeftDoor) _layoutCode += 1;
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
        var pos = transform.position;
        
        Gizmos.color = topLeftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 20, 0), pos + new Vector3(20, 20, 0));

        Gizmos.color = topRightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 20, 0), pos + new Vector3(40, 20, 0));

        Gizmos.color = bottomLeftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(20, 0, 0));
        
        Gizmos.color = bottomRightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 0, 0), pos + new Vector3(40, 0, 0));

        Gizmos.color = leftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(0, 20, 0));

        Gizmos.color = rightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(40, 0, 0), pos + new Vector3(40, 20, 0));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
