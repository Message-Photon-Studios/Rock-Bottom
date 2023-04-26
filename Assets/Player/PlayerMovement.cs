using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// This class controls the players movement and keeps track of player states such as it being rooted, falling or in the air. 
/// The class does also controll the cameras focus point and therefore controlls the camera checks. 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed; // Base movement speed for the player in air and on ground
    [SerializeField] float jumpPower; //The initial boost power for the jump. Increasing this will increse the jumpheight and jump speed but decrease controll
    [SerializeField] float leapPower; //This detemines the extra forward speed of the double jump
    [SerializeField] float wallJumpPower;
    [SerializeField] float jumpJetpack; //A small extra power over time for the jump that alows the player to controll the height of the jump
    [SerializeField] float jumpFalloff; //The falloff power of the jump jetpack

    /*
    * The jumpJetpack and the jumpFalloff does controll the extra force over time for the players jump that allows the player to controll the heigh of the jump.
    * The floats does both controll the power of the jetpack and the max time of the jetpack. Increasing both variables will increase the power of the jetpack
    * without increasing its duration. Increasing only the jumpJetpack will increase the power and the time of the jetpack while increasing only the 
    * jumpFalloff will decrease the time and the power of the jetpack. This does also work in reverse for decreasing variables.
    */

    [SerializeField] InputActionReference walkAction, jumpAction, belowCheckAction, aboveCheckAction, lockCamera; //Input actiuons for controlling the movement and camera checks
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform focusPoint; //The point tha the camera will try to focus on
    [SerializeField] float checkPointY;
    [SerializeField] CapsuleCollider2D playerCollider;

    /// <summary>
    /// The time that the player has spent in the air. Is 0 if the player is standing on the ground.
    /// </summary>
    public float airTime {get; private set;}

    /// <summary>
    /// The time that the player has spent falling. Is 0 if the player are not falling.
    /// </summary>
    public float fallTime {get; private set;}

    /// <summary>
    /// The direction that the player is currently facing. 1 means facing right and -1 means facing left.
    /// </summary>
    public int lookDir {get; private set;} = 1;

    /// <summary>
    /// Controls if the player is rooted 
    /// </summary>
    public MovementRoot movementRoot = new MovementRoot(new string[0]);
    private float focusPointNormalY;
    float movement = 0;
    private bool doubleJumpActive = false;
    private float jump;
    
    Action<InputAction.CallbackContext> movementRootTrue;
    Action<InputAction.CallbackContext> movementRootFalse;
    Action<InputAction.CallbackContext> checkBelow;
    Action<InputAction.CallbackContext> checkAbove;
    Action<InputAction.CallbackContext> checkCancle;


    #region Setup
    private void OnEnable() {
        movementRootTrue = (InputAction.CallbackContext ctx) => {movementRoot.SetRoot("CameraRoot", true);};
        movementRootFalse = (InputAction.CallbackContext ctx) => {movementRoot.SetRoot("CameraRoot", false);};
        checkBelow = (InputAction.CallbackContext ctx) => {CheckBelowStart();};
        checkAbove = (InputAction.CallbackContext ctx) => {CheckAboveStart();};
        checkCancle = (InputAction.CallbackContext ctx) => {CheckCancel();};
        
        jumpAction.action.started += Jump;
        jumpAction.action.canceled += JumpCancel;
        belowCheckAction.action.performed += checkBelow;
        belowCheckAction.action.canceled += checkCancle;
        aboveCheckAction.action.started += checkAbove;
        aboveCheckAction.action.canceled += checkCancle;
        lockCamera.action.started += movementRootTrue;
        lockCamera.action.canceled += movementRootFalse;
    }

    private void OnDisable() {
        jumpAction.action.started -= Jump;
        jumpAction.action.canceled -= JumpCancel;
        belowCheckAction.action.started -= checkBelow;
        belowCheckAction.action.canceled -= checkCancle;
        aboveCheckAction.action.started -= checkAbove;
        aboveCheckAction.action.canceled -= checkCancle;
        lockCamera.action.started -= movementRootTrue;
        lockCamera.action.canceled -= movementRootFalse;
    }

    void Start()
    {
        focusPointNormalY = focusPoint.localPosition.y;
    }

    #endregion

    #region Jump

    /// <summary>
    /// Make the character jump or double jump
    /// </summary>
    void Jump(InputAction.CallbackContext ctx)
    {
        if(movementRoot.rooted) return;

        if(IsGrounded())
        {
            body.AddForce(new Vector2(movement, 0));
            body.AddForce(Vector2.up * jumpPower);
            jump = jumpJetpack;
            doubleJumpActive = false;
            return;
        } else if(IsGrappeling())
        {
            bool wallRight = Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right, 1f, 3);
            body.AddForce(new Vector2((wallRight?-1:1)*wallJumpPower, 0));
            body.AddForce(Vector2.up * jumpPower);
            jump = jumpJetpack;
            doubleJumpActive = false;
        } else if(!doubleJumpActive)
        {
            body.AddForce(new Vector2(movement*leapPower, 0));
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.up * jumpPower);
            doubleJumpActive = true;
            jump = jumpJetpack;
        }
    }

    /// <summary>
    /// Cancels the jump
    /// </summary>
    void JumpCancel(InputAction.CallbackContext ctx)
    {
        jump = 0;
    }

    #endregion

    #region Camera Check
    void CheckBelowStart()
    {
        if(focusPoint == null && movementRoot.totalRoot) return;
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, -checkPointY, focusPoint.localPosition.z);
    }

    void CheckAboveStart()
    {
        if(focusPoint == null && movementRoot.totalRoot) return;
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, checkPointY, focusPoint.localPosition.z);
    }

    void CheckCancel()
    {
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, focusPointNormalY, focusPoint.localPosition.z);
    }

    #endregion

    #region Collision checks
    private bool IsGrounded()
    {  
        return  Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, 3) ||
                Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, 3);
    }

    private bool HitCeling ()
    {
        return Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.up, 1f, 3) ||
                Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.up, 1f, 3);
    }

    private bool IsGrappeling()
    {
        return  (!Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down*1.3f, 1f, 3) ||
                !Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.down*1.3f, 1f, 3)) &&
                ((Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right*0.1f, 1f, 3))  ||
                (Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.left*0.1f, 1f, 3)));
    }
   
    #endregion

    private void FixedUpdate() {
        movementRoot.UpdateTimers();

        float walkDir = walkAction.action.ReadValue<float>();

        if(movementRoot.totalRoot) walkDir = 0;

        if(walkDir < 0 && lookDir > 0 ) Flip();
        else if(walkDir > 0 && lookDir < 0) Flip();
        movement = movementSpeed * walkDir;

        if(jump > 0)
            jump -= jumpFalloff * Time.fixedDeltaTime;
        else if(jump < 0)
            jump = 0;

        if(IsGrounded())
        {
            playerAnimator.SetInteger("velocityY", 0);

            airTime = 0;
            fallTime = 0;
            if(!movementRoot.rooted) body.velocity = new Vector2(movement, body.velocity.y);
            if(doubleJumpActive) doubleJumpActive = false;
            if(!playerAnimator.GetBool("walking") && body.velocity.x != 0) playerAnimator.SetBool("walking", true);
            else if(playerAnimator.GetBool("walking") && body.velocity.x == 0) playerAnimator.SetBool("walking", false);
    
        } else
        {
            if(playerAnimator.GetBool("walking")) playerAnimator.SetBool("walking", false);
            if(!movementRoot.rooted)
                body.AddForce(new Vector2(movement*10, 0));
            airTime += Time.fixedDeltaTime;

            if(HitCeling())
            {
                jump = 0;
            }

            if(body.velocity.y > 0f)
            {
                playerAnimator.SetInteger("velocityY", 1);
            } else
            {
                playerAnimator.SetInteger("velocityY", -1);
                fallTime += Time.fixedDeltaTime;
            }
        }

        if(IsGrappeling())
        {
            bool wallRight = Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right, 1f, 3);
            if(wallRight == spriteRenderer.flipX) Flip();
            playerAnimator.SetBool("grapple", true);
            fallTime = 0;
            airTime = 0;
            CheckCancel();
            if(body.velocity.y < 0)
            {
                body.velocity = new Vector2(body.velocity.x, -2);
            }
        } else
        {
            playerAnimator.SetBool("grapple", false);
        }
       
        if(!movementRoot.rooted)
            body.AddForce(new Vector2(0,jump));

    }

    /// <summary>
    /// Flips the player correctly
    /// </summary>
    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        lookDir = (!spriteRenderer.flipX)?1:-1 ;
        focusPoint.localPosition = new Vector3(-focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
    }

}

#region Movement Root

/// <summary>
/// A struct that propperly keeps track of wheter the player is rooted. 
/// </summary>
public class MovementRoot
{
    /// <summary>
    /// If it is true then the player should be rooted. 
    /// </summary>
    public bool rooted {get; private set;}
    /// <summary>
    /// If the player is total rooted then the camera can't be moved as well
    /// </summary>
    /// <value></value>
    public bool totalRoot {get; private set;}
    private Dictionary<string, float> effects;

    public MovementRoot(string[] rootEffects)
    {
        effects = new Dictionary<string, float>();
        rooted = false;

        for (int i = 0; i < rootEffects.Length; i++)
        {
            SetRoot(rootEffects[i], true);
        }
    }

    /// <summary>
    /// Call this method to root or unroot the player. The player will only be unrooted completely if it has no effects that roots it. 
    /// </summary>
    /// <param name="effect">The effect that roots the player. The player stops being rooted if all effects are canceled</param>
    /// <param name="root">If it is true the effect will be activated and if it is false it will be canceled</param>
    public void SetRoot(string effect, bool root)
    {
        if(root)
        {
            if(!effects.ContainsKey(effect))
                effects.Add(effect, -1);
        } else
        {
            if(effects.ContainsKey(effect))
                effects.Remove(effect);
        }

        rooted = effects.Count > 0;
    }

    /// <summary>
    /// Call this method to root or unroot the player. The player will only be unrooted completely if it has no effects that roots it. 
    /// This does also set the total root. Setting the total root will disable camera movement completely
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="root"></param>
    public void SetTotalRoot(string effect, bool root)
    {
        if(root)
        {
            if(!effects.ContainsKey(effect))
                effects.Add(effect, -1);
        } else
        {
            if(effects.ContainsKey(effect))
                effects.Remove(effect);
        }

        totalRoot = root;
        rooted = effects.Count > 0;
    }

    /// <summary>
    /// Add a timed root. The root will be automaticaly removed after the time has ended.
    /// If a root with the same name exists then then this time will be added to it.
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="time"></param>
    public void SetRoot(string effect, float time)
    {
        if(!effects.ContainsKey(effect))
            effects.Add(effect, time);
        else
            effects[effect] += time;
        rooted = effects.Count > 0;
    }

    /// <summary>
    /// Updates the timed roots
    /// </summary>
    public void UpdateTimers()
    {
        Dictionary<string, float> temp = new Dictionary<string, float>(effects);


        foreach (KeyValuePair<string, float> effect in temp)
        {
            if (effect.Value > 0)
            {
                float newTime = effect.Value - Time.deltaTime;
                if (newTime <= 0)
                {
                    effects.Remove(effect.Key);
                }
                else
                {
                    effects[effect.Key] = newTime;
                }
            }
        }

        rooted = effects.Count > 0;
    }

    /// <summary>
    /// Completely cancles the root and removes all effects
    /// </summary>
    public void UnrootCompletely()
    {
        rooted = false;
        effects = new Dictionary<string, float>();
    }
}

#endregion