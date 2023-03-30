using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float leapPower;
    [SerializeField] float jumpJetpack;
    [SerializeField] float jumpFalloff;
    [SerializeField] InputActionReference walkAction, jumpAction, belowCheckAction, aboveCheckAction, lockCamera;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform focusPoint;
    [SerializeField] float checkPointY;
    [SerializeField] CapsuleCollider2D playerCollider;
    //[SerializeField] Sprite normalSprite;
    //[SerializeField] Sprite jumpSprite;
    //[SerializeField] Sprite fallSprite;
    private float airTime;

    public int lookDir {get; private set;} = 1;

    public MovementRoot movementRoot = new MovementRoot(new string[0]);
    private float focusPointNormalY;
    float movement = 0;
    private bool doubleJumpActive = false;
    private float jump;
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
        }

        if(!doubleJumpActive)
        {
             body.AddForce(new Vector2(movement*leapPower, 0));
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.up * jumpPower);
            doubleJumpActive = true;
            jump = jumpJetpack;
        }
    }

    void JumpCancel()
    {
        jump = 0;
    }

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

    private void FixedUpdate() {
        float walkDir = walkAction.action.ReadValue<float>();

        if(walkDir < 0 && lookDir > 0 ) Flip();
        else if(walkDir > 0 && lookDir < 0) Flip();
        movement = movementSpeed * walkDir;

        if(jump > 0)
            jump -= jumpFalloff * Time.fixedDeltaTime;
        else if(jump < 0)
            jump = 0;

        
        if(airTime > .8f)
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
            }
        }
       
        if(!movementRoot.rooted)
            body.AddForce(new Vector2(0,jump));

    }

    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        lookDir = (!spriteRenderer.flipX)?1:-1 ;
        focusPoint.localPosition = new Vector3(-focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
    }

}

public struct MovementRoot
{
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
}
