using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] float fallTimeCamera; //The time the player needs to fall untill the camera moves to checking below the player
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

    #region Setup
    private void OnEnable() {
        jumpAction.action.started += (_) => {Jump();};
        jumpAction.action.canceled += (_) => {JumpCancel();};
        belowCheckAction.action.performed += (_) => {CheckBelowStart();};
        belowCheckAction.action.canceled += (_) => {CheckCancel();};
        aboveCheckAction.action.started += (_) => {CheckAboveStart();};
        aboveCheckAction.action.canceled += (_) => {CheckCancel();};
        lockCamera.action.started += (_) => {movementRoot.SetRoot("CameraRoot", true);};
        lockCamera.action.canceled += (_) => {movementRoot.SetRoot("CameraRoot", false);};
    }

    private void OnDisable() {
        jumpAction.action.started -= (_) => {Jump();};
        jumpAction.action.canceled -= (_) => {JumpCancel();};
        belowCheckAction.action.started -= (_) => {CheckBelowStart();};
        belowCheckAction.action.canceled -= (_) => {CheckCancel();};
        aboveCheckAction.action.started -= (_) => {CheckAboveStart();};
        aboveCheckAction.action.canceled -= (_) => {CheckCancel();};
        lockCamera.action.started -= (_) => {movementRoot.SetRoot("CameraRoot", true);};
        lockCamera.action.canceled -= (_) => {movementRoot.SetRoot("CameraRoot", false);};
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
    void Jump()
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
    void JumpCancel()
    {
        jump = 0;
    }

    #endregion

    #region Camera Check
    void CheckBelowStart()
    {
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, -checkPointY, focusPoint.localPosition.z);
    }

    void CheckAboveStart()
    {
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
        return  (!Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, 3) ||
                !Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, 3)) &&
                ((Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right, 1f, 3) && movement > 0)  ||
                (Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.left, 1f, 3) && movement < 0));
    }
    #endregion

    private void FixedUpdate() {
        float walkDir = walkAction.action.ReadValue<float>();

        if(walkDir < 0 && lookDir > 0 ) Flip();
        else if(walkDir > 0 && lookDir < 0) Flip();
        movement = movementSpeed * walkDir;

        if(jump > 0)
            jump -= jumpFalloff * Time.fixedDeltaTime;
        else if(jump < 0)
            jump = 0;

        
        if(fallTime > fallTimeCamera)
        {
            if(body.velocity.y < 0)
                CheckBelowStart();
            if(IsGrounded())
            {
                CheckCancel();
            }
        }

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
            playerAnimator.SetBool("grapple", true);
            fallTime = 0;
            airTime = 0;
            CheckCancel();
            if(body.velocity.y < 0)
            {
                body.velocity = new Vector2(body.velocity.x, 0);
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
public struct MovementRoot
{
    /// <summary>
    /// If it is true then the player should be rooted. 
    /// </summary>
    public bool rooted {get; private set;}
    private List<string> effects;

    public MovementRoot(string[] rootEffects)
    {
        effects = new List<string>();
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
            if(!effects.Exists(effector => effector.Equals(effect)))
                effects.Add(effect);
        } else
        {
            if(effects.Exists(effect => effect.Equals(effect)))
                effects.Remove(effect);
        }

        rooted = effects.Count > 0;
    }

    /// <summary>
    /// Completely cancles the root and removes all effects
    /// </summary>
    public void UnrootCompletely()
    {
        rooted = false;
        effects = new List<string>();
    }
}

#endregion