using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This point follows the players aim point but reduces the movement in some extreme circumstances
/// </summary>
public class CameraFocus : MonoBehaviour
{
    [SerializeField] float maxYpos; //The maximum position the player can be above the camera before the camera snaps to the player
    [SerializeField] float minYpos; //The minimum position the player can be belove the camera before the players 
    [SerializeField] float maxXspeed; //The max horizontal speed the camera can have. Usefull for controlling the speed of the horizontal flip
    [SerializeField] Vector2 deadZone;
    [SerializeField] Transform aim; //The point the camera focus is following.
    [SerializeField] PlayerMovement playerMovement;
    float x = 0;
    float y = 0;
    bool fallCamera = false;
    void OnEnable()
    {
        x = aim.position.x;
        y = aim.position.y;
    }

    public void SetStartLevel()
    {
        OnEnable();
    }

    void FixedUpdate()
    {
        if(aim.position.x > deadZone.x+transform.position.x || aim.position.x < -deadZone.x+transform.position.x)
        {
            if(Mathf.Abs(aim.position.x-x) > maxXspeed)
            {
                x += (aim.position.x-x < 0)? -maxXspeed:maxXspeed;
            } else
                x = aim.position.x;
        }

        if(playerMovement.airTime <= 0.01f)
        {
            fallCamera = false;
            if(aim.position.y > deadZone.y+transform.position.y || aim.position.y < -deadZone.y+transform.position.y)
                y = aim.position.y;
        } else if(transform.position.y-aim.position.y > maxYpos || aim.position.y - transform.position.y < minYpos || fallCamera)
        {
            y = aim.position.y;
            fallCamera = true;
        }
        
        /*if(aim.position.x > deadZone.x+transform.position.x || aim.position.x < -deadZone.y+transform.position.x)
            transform.position = new 
        if(focusPoint.position.y > deadZone.y+transform.position.y || focusPoint.position.y < -deadZone.y+transform.position.y)
            moveVerticalPos = Vector3.Slerp(transform.position, focusPoint.position, ((focusPoint.position.y > transform.position.y)? verticalSpeedUp:verticalSpeedDown)*Time.fixedDeltaTime);*/
        transform.position = new Vector3(x,y,aim.position.z);
    }
}
