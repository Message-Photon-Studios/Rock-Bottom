using System.Collections.Generic;
using UnityEngine;

public class TallRoomData : MonoBehaviour
{
    public bool topDoor;
    public bool bottomDoor;
    public bool leftTopDoor;
    public bool leftBottomDoor;
    public bool rightTopDoor;
    public bool rightBottomDoor;

    [Space(10)]
    public bool renderGizmos;
    
    [HideInInspector]
    public int layoutCode
    {
        get
        {
            var _layoutCode = 0;
            if (topDoor) _layoutCode += 32;
            if (bottomDoor) _layoutCode += 16;
            if (leftTopDoor) _layoutCode += 8;
            if (leftBottomDoor) _layoutCode += 4;
            if (rightTopDoor) _layoutCode += 2;
            if (rightBottomDoor) _layoutCode += 1;
            return _layoutCode;
        }
    }
    
    [HideInInspector]
    public int mirroredLayoutCode
    {
        get
        {
            var _layoutCode = 0;
            if (topDoor) _layoutCode += 32;
            if (bottomDoor) _layoutCode += 16;
            if (rightTopDoor) _layoutCode += 8;
            if (rightBottomDoor) _layoutCode += 4;
            if (leftTopDoor) _layoutCode += 2;
            if (leftBottomDoor) _layoutCode += 1;
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
        
        Gizmos.color = topDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 40, 0), pos + new Vector3(20, 40, 0));

        Gizmos.color = bottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(20, 0, 0));
        
        Gizmos.color = rightBottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 0, 0), pos + new Vector3(20, 20, 0));

        Gizmos.color = leftBottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(0, 20, 0));

        Gizmos.color = rightTopDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 20, 0), pos + new Vector3(20, 40, 0));

        Gizmos.color = leftTopDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 20, 0), pos + new Vector3(0, 40, 0));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
