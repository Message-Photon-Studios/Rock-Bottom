using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float verticalSpeed;  
    [SerializeField] Transform focusPoint;

    private void FixedUpdate() {
        Vector3 movePos = Vector3.Slerp(transform.position, focusPoint.position, speed*Time.fixedDeltaTime);
        Vector3 moveVerticalPos = Vector3.Slerp(transform.position, focusPoint.position, verticalSpeed*Time.fixedDeltaTime);

        transform.position = new Vector3(movePos.x, moveVerticalPos.y, transform.position.z);
    }
}
