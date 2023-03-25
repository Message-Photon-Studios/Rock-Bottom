using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRoomData : MonoBehaviour
{
    public bool leftTopDoor;
    public bool topLeftDoor;
    public bool rightTopDoor;
    public bool topRightDoor;
    public bool leftBottomDoor;
    public bool bottomLeftDoor;
    public bool rightBottomDoor;
    public bool bottomRightDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.color = leftTopDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 20, 0), pos + new Vector3(0, 40, 0));
        
        Gizmos.color = topLeftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 40, 0), pos + new Vector3(20, 40, 0));

        Gizmos.color = rightTopDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(40, 20, 0), pos + new Vector3(40, 40, 0));

        Gizmos.color = topRightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 40, 0), pos + new Vector3(40, 40, 0));

        Gizmos.color = bottomLeftDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(20, 0, 0));
        
        Gizmos.color = bottomRightDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(20, 0, 0), pos + new Vector3(40, 0, 0));

        Gizmos.color = leftBottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(0, 0, 0), pos + new Vector3(0, 20, 0));

        Gizmos.color = rightBottomDoor ? Color.green : Color.white;
        Gizmos.DrawLine(pos + new Vector3(40, 0, 0), pos + new Vector3(40, 20, 0));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
