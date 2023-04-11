using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [SerializeField] float maxYdist;
    [SerializeField] float maxXspeed;
    [SerializeField] Transform aim;
    [SerializeField] PlayerMovement playerMovement;
    float x = 0;
    float y = 0;
    bool fallCamera = false;
    void OnEnable()
    {
        x = aim.position.x;
        y = aim.position.y;
    }

    void FixedUpdate()
    {
        if(Mathf.Abs(aim.position.x-x) > maxXspeed)
        {
            x += (aim.position.x-x < 0)? -maxXspeed:maxXspeed;
        } else
            x = aim.position.x;

        if(playerMovement.airTime <= 0.01f)
        {
            y = aim.position.y;
            fallCamera = false;
        } else if(Mathf.Abs(transform.position.y - aim.position.y) > maxYdist || fallCamera)
        {
            y = aim.position.y;
            fallCamera = true;
        }
        
        transform.position = new Vector3(x,y,aim.position.z);
    }
}
