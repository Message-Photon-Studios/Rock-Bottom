using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// This class controls the cameras movement
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed; //The horizontal speed of the camera
    [SerializeField] float verticalSpeedUp; //The vertical speed of the camera going up
    [SerializeField] float verticalSpeedDown; //The vertical speed of the camera going dow
    [SerializeField] Transform focusPoint; //The point that the camera will try and follow

    private void Start()
    {
        var ratio = Screen.height / 1080.0;
        if (Math.Floor(ratio) > 0)
            ratio /= Math.Floor(ratio);
        ratio *= 100;
        
        var pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        pixelPerfectCamera.assetsPPU = (int)Math.Floor(ratio);
    }

    private void FixedUpdate() {
        Vector3 movePos = Vector3.Slerp(transform.position, focusPoint.position, speed*Time.fixedDeltaTime);
        Vector3 moveVerticalPos = Vector3.Slerp(transform.position, focusPoint.position, ((focusPoint.position.y > transform.position.y)? verticalSpeedUp:verticalSpeedDown)*Time.fixedDeltaTime);

        transform.position = new Vector3(movePos.x, moveVerticalPos.y, transform.position.z);
    }
}
