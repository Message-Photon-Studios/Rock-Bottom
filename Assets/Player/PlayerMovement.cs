using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Diagnostics.Tracing;

/// <summary>
/// This class controls the players movement and keeps track of player states such as it being rooted, falling or in the air. 
/// The class does also controll the cameras focus point and therefore controlls the camera checks. 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed; // Base movement speed for the player in air and on ground
    [SerializeField] float climbSpeed; //The speed that the player is climbing with
    [SerializeField] float jumpPower; //The initial boost power for the jump. Increasing this will increse the jumpheight and jump speed but decrease controll
    [SerializeField] float leapPower; //This detemines the extra forward speed of the double jump
    [SerializeField] float wallJumpPower; //The power of the jump when jumping of wall
    [SerializeField] float wallStickPower; //The power that pushes the player towards the wall when climbing.
    [SerializeField] float jumpJetpack; //A small extra power over time for the jump that alows the player to controll the height of the jump
    [SerializeField] float jumpFalloff; //The falloff power of the jump jetpack
    [SerializeField] float coyoteTime; //A small second affter leaving a platform you can still jump as normal. 

    /*
    * The jumpJetpack and the jumpFalloff does controll the extra force over time for the players jump that allows the player to controll the heigh of the jump.
    * The floats does both controll the power of the jetpack and the max time of the jetpack. Increasing both variables will increase the power of the jetpack
    * without increasing its duration. Increasing only the jumpJetpack will increase the power and the time of the jetpack while increasing only the 
    * jumpFalloff will decrease the time and the power of the jetpack. This does also work in reverse for decreasing variables.
    */

    [SerializeField] InputActionReference walkAction, jumpAction, lookAction; //Input actiuons for controlling the movement and camera checks
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform focusPoint; //The point tha the camera will try to focus on
    [SerializeField] float aimFocusMaxX;
    [SerializeField] float aimFocusAcceleration;
    [SerializeField] float checkPointY;
    [SerializeField] CapsuleCollider2D playerCollider;
    [SerializeField] ParticleSystem dustParticles;
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] ParticleSystem wallParticles;
    [SerializeField] ParticleSystem wallJumpParticles;

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

    private float coyoteTimer = 0;
    private float coyoteTimerWall = 0;

    private bool wasClimbing = false;
    float walkDir = 0f;

    private int beforeClimbLookDir = 0;

    [HideInInspector] public bool inAttackAnimation = false;

    private bool climbCeilingDetected = false;

    Vector3 originalFocusPointPos;

    [SerializeField] PlayerSounds playerSounds;


    Action<InputAction.CallbackContext> checkAction;
    Action<InputAction.CallbackContext> checkCancle;

    [HideInInspector] public bool isCheckingY = false; //Is true when player checks above or below
    #region Setup
    private void OnEnable() {
        originalFocusPointPos = new Vector3(focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
        movementRoot.SetTotalRoot("loading", true);
        checkAction = (InputAction.CallbackContext ctx) => {
            if(lookAction.action.ReadValue<float>() < 0f)
            {
                CheckBelowStart();
            }
            else if(lookAction.action.ReadValue<float>() > 0f)
                CheckAboveStart();
        };

        checkCancle = (InputAction.CallbackContext ctx) => {CheckCancel();};
        
        jumpAction.action.started += Jump;
        jumpAction.action.canceled += JumpCancel;
        lookAction.action.performed += checkAction;
        lookAction.action.canceled += checkCancle;
    }

    private void OnDisable() {
        jumpAction.action.started -= Jump;
        jumpAction.action.canceled -= JumpCancel;
        lookAction.action.performed -= checkAction;
        lookAction.action.canceled -= checkCancle;
    }

    void Start()
    {
        focusPointNormalY = focusPoint.localPosition.y;
    }

    #endregion

    #region Jump

    void Jump(InputAction.CallbackContext ctx)
    {
        Jump();
    }

    /// <summary>
    /// Make the character jump or double jump
    /// </summary>
    void Jump()
    {
        
        if(movementRoot.rooted) return;
        body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        Physics2D.IgnoreLayerCollision(GameManager.instance.maskLibrary.playerLayer, GameManager.instance.maskLibrary.platformLayer, true);
        wasClimbing = false;
        if (IsGrounded() || coyoteTimer > 0)
        {
            body.AddForce(new Vector2(movement, 0));
            body.AddForce(Vector2.up * jumpPower);
            jump = jumpJetpack;
            doubleJumpActive = false;
            playerSounds.PlayJump();
            jumpParticles.Play();
            dustParticles.Stop();
            coyoteTimer = 0;
            return;
        } else if(IsGrappeling() || coyoteTimerWall > 0)
        {
            body.AddForce(Vector2.up * jumpPower);

            bool wallRight = Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right, 1f, 3);
            body.AddForce(new Vector2((wallRight?-1:1)*wallJumpPower, 0)); 
            jump = jumpJetpack;
            playerSounds.PlayJump();
            wallJumpParticles.transform.eulerAngles = wallRight ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
            wallJumpParticles.transform.localPosition = wallRight ? new Vector3(0.44f, 0.202f, 0f) : new Vector3(-0.44f, 0.202f, 0f);
            wallJumpParticles.Play();
            coyoteTimerWall = 0;
        } else if(!doubleJumpActive)
        {
            body.AddForce(new Vector2(movement*leapPower, 0));
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.up * jumpPower);
            doubleJumpActive = true;
            jump = jumpJetpack;
            playerSounds.PlayJump();
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
    float checkDownTimer = 0;
    void CheckBelowStart()
    {
        checkDownTimer = .2f;
        Physics2D.IgnoreLayerCollision(GameManager.instance.maskLibrary.playerLayer, GameManager.instance.maskLibrary.platformLayer, true);
        if(focusPoint == null && movementRoot.totalRoot) return;
        if(IsGrappeling() || !IsGrounded() || IsOnPlatform()) return;
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, -checkPointY, focusPoint.localPosition.z);
        isCheckingY = true;
    }

    void CheckAboveStart()
    {
        if(focusPoint == null && movementRoot.totalRoot) return;
        if(IsGrappeling() || !IsGrounded()) return;
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, checkPointY, focusPoint.localPosition.z);
        isCheckingY = true;
    }

    void CheckCancel()
    {
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, focusPointNormalY, focusPoint.localPosition.z);
        isCheckingY = false;
    }

    #endregion

    #region Collision checks
    public bool IsGrounded()
    {  
        bool ret =  Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlyGround) ||
                    Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlyGround);
        playerAnimator.SetBool("grounded", ret);
        return ret;
    }

    public bool IsOnPlatform() 
    {
        return Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlyPlatforms) &&
        Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlyPlatforms);
    }

    private bool HitCeling ()
    {
        return  Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.up, .6f, GameManager.instance.maskLibrary.onlySolidGround()) ||
                Physics2D.Raycast(transform.position-Vector3.right* playerCollider.size.x/2, Vector2.up, .6f, GameManager.instance.maskLibrary.onlySolidGround());
    }

    public bool IsGrappeling()
    {
        if(walkDir != lookDir && walkDir != 0) return false;

        return  (!Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 2.1f, GameManager.instance.maskLibrary.onlyGround) && 
                Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/4, Vector2.right, .5f, GameManager.instance.maskLibrary.onlyGround)) ||
                (!Physics2D.Raycast(transform.position+Vector3.left* playerCollider.size.x/2, Vector2.down, 2.1f, GameManager.instance.maskLibrary.onlyGround) &&
                Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/4, Vector2.left, .5f, GameManager.instance.maskLibrary.onlyGround)) ||
                ((wasClimbing) && (
                    (!Physics2D.Raycast(transform.position+Vector3.right* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlySolidGround()) && 
                    Physics2D.Raycast((Vector2)transform.position+Vector2.down* playerCollider.size.y/2+playerCollider.offset, Vector2.right, .7f, GameManager.instance.maskLibrary.onlyGround)) ||
                    (!Physics2D.Raycast(transform.position+Vector3.left* playerCollider.size.x/2, Vector2.down, 1f, GameManager.instance.maskLibrary.onlySolidGround()) &&
                    Physics2D.Raycast((Vector2)transform.position+Vector2.down* playerCollider.size.y/2+playerCollider.offset, Vector2.left, .7f, GameManager.instance.maskLibrary.onlyGround))  
                ));
    }
   
    #endregion

    #region Update Loop
    bool wallRight = false;
    private void FixedUpdate() {
        
        if(jump > 0 || IsGrappeling()) Physics2D.IgnoreLayerCollision(GameManager.instance.maskLibrary.playerLayer, GameManager.instance.maskLibrary.platformLayer, true);
        else if(checkDownTimer <= 0) Physics2D.IgnoreLayerCollision(GameManager.instance.maskLibrary.playerLayer, GameManager.instance.maskLibrary.platformLayer, false);

        if(checkDownTimer > 0) checkDownTimer -= Time.deltaTime;
        movementRoot.UpdateTimers();

        if(inAttackAnimation) return;

        walkDir = walkAction.action.ReadValue<float>();

        if(movementRoot.totalRoot) walkDir = 0;

        if(walkDir < 0 && lookDir > 0 ) Flip();
        else if(walkDir > 0 && lookDir < 0) Flip();
        movement = movementSpeed * walkDir;

        if(jump > 0)
            jump -= jumpFalloff * Time.fixedDeltaTime;
        else if(jump < 0)
            jump = 0;
        if ((walkDir == 0 || IsGrappeling()) && focusPoint.localPosition.x != 0 )
        {
            Vector3 aimPosTo = Vector3.Slerp(focusPoint.localPosition, new Vector3(originalFocusPointPos.x, focusPoint.localPosition.y, focusPoint.localPosition.z) , aimFocusAcceleration*Time.fixedDeltaTime);
            focusPoint.localPosition = new Vector3(aimPosTo.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
            //focusPoint.localPosition += Vector3.Normalize(originalFocusPointPos-focusPoint.localPosition) * aimFocusAcceleration * Time.fixedDeltaTime;
        }
        if(IsGrounded())
        {
            if(walkDir != 0 && focusPoint.localPosition.x < aimFocusMaxX && focusPoint.localPosition.x > -aimFocusMaxX)
            {
                Vector3 aimPosTo = Vector3.Slerp(focusPoint.localPosition, new Vector3(aimFocusMaxX*lookDir, focusPoint.localPosition.y, focusPoint.localPosition.z), aimFocusAcceleration*Time.fixedDeltaTime);
                focusPoint.localPosition = new Vector3(aimPosTo.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
                //focusPoint.localPosition += new Vector3(aimFocusAcceleration*lookDir*Time.fixedDeltaTime, 0 , 0);
            } 
            GetComponent<PlayerCombatSystem>().SetPlayerGrounded();
            coyoteTimer = coyoteTime;
            playerAnimator.SetInteger("velocityY", 0);

            doubleJumpActive = false;

            airTime = 0;
            fallTime = 0;
            if(!movementRoot.rooted) body.velocity = new Vector2(movement, body.velocity.y);
            if(doubleJumpActive) doubleJumpActive = false;
            if(!playerAnimator.GetBool("walking") && body.velocity.x != 0)
            {
                dustParticles.Play();
                playerAnimator.SetBool("walking", true);
            }
            else if(playerAnimator.GetBool("walking") && body.velocity.x == 0)
            {
                dustParticles.Stop();
                playerAnimator.SetBool("walking", false);
            }
    
        } else
        {
            coyoteTimer -= Time.fixedDeltaTime;
            if(playerAnimator.GetBool("walking"))
            {
                dustParticles.Stop();
                playerAnimator.SetBool("walking", false);
            }
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
            if(!wasClimbing) beforeClimbLookDir = lookDir;

            coyoteTimerWall = coyoteTime;
            GetComponent<PlayerCombatSystem>().SetPlayerGrounded();
            wallRight = Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/2, Vector2.right, .5f, 3)||
                        Physics2D.Raycast(transform.position+Vector3.down* playerCollider.size.y/4, Vector2.right, .5f, 3);
            if(wallRight == spriteRenderer.flipX) Flip();
            playerAnimator.SetBool("grapple", true);
            fallTime = 0;
            airTime = 0;
            CheckCancel();
            doubleJumpActive = false;
            wallParticles.transform.localPosition = wallRight ? new Vector3(0.692f, 0.592f, 0f) : new Vector3(-0.692f, 0.592f, 0f);

            
            wasClimbing = true;
            if(walkDir != lookDir && walkDir != 0)
            {
               ReleaseWall();
            }
            else if(walkDir == lookDir)
            {
                playerAnimator.SetInteger("velocityY", 1);
                wallParticles.Stop();

                if(HitCeling())
                {
                    if(!climbCeilingDetected)
                    {
                        body.constraints |= RigidbodyConstraints2D.FreezePositionY;   
                        body.velocity = new Vector2(body.velocity.x+wallStickPower*lookDir, 0);
                        climbCeilingDetected = true;
                    }
                } else
                {
                    body.velocity = new Vector2(body.velocity.x+wallStickPower*lookDir, climbSpeed);
                    climbCeilingDetected = false;
                }
            }
            else if(body.velocity.y < 0)
            {
                body.velocity = new Vector2(body.velocity.x + wallStickPower*lookDir, -2);
                playerAnimator.SetInteger("velocityY", -1);
                wallParticles.Play();
            } else if(!inAttackAnimation)
            {   
                body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            }
        } else
        {
            coyoteTimerWall -= Time.fixedDeltaTime;
            playerAnimator.SetBool("grapple", false);
            wallParticles.Stop();
            
            if(wasClimbing && !inAttackAnimation)
            {
                body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
                if(beforeClimbLookDir != lookDir) Flip();
                wasClimbing = false;
            }
        }
       
        if(!movementRoot.rooted)
            body.AddForce(new Vector2(0,jump));
    }

    #region Climbing

    public void ReleaseWall()
    {
        wasClimbing = false;
        playerAnimator.SetInteger("velocityY", -1);
        body.velocity = new Vector2(body.velocity.x-5*lookDir, 0);
        wallParticles.Stop();
        body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        if(wallRight != spriteRenderer.flipX) Flip();
        beforeClimbLookDir = lookDir;
    }

    public void WallAttackLock()
    {
        body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        body.velocity = new Vector2(body.velocity.x-5*lookDir, 0);
        if(wallRight != spriteRenderer.flipX) Flip();
        beforeClimbLookDir = lookDir;
    }

    #endregion

    #endregion

    /// <summary>
    /// Flips the player correctly
    /// </summary>
    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        lookDir = (!spriteRenderer.flipX)?1:-1 ;
        //focusPoint.localPosition = new Vector3(-focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
        if(IsGrounded())
            playerAnimator.SetTrigger("turn");
        GetComponent<PlayerCombatSystem>().FlipDefaultAttack();
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