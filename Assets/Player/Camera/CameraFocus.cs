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
        } else if(transform.position.y-aim.position.y > maxYpos || aim.position.y - transform.position.y < minYpos || fallCamera)
        {
            y = aim.position.y;
            fallCamera = true;
        }
        
        transform.position = new Vector3(x,y,aim.position.z);
    }
}
