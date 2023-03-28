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
    [SerializeField] InputActionReference walkAction, jumpAction, belowCheckAction;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform focusPoint;
    [SerializeField] float checkBelowPoint;
    [SerializeField] CapsuleCollider2D playerCollider;
    //[SerializeField] Sprite normalSprite;
    //[SerializeField] Sprite jumpSprite;
    //[SerializeField] Sprite fallSprite;


    private float airTime;

    private float focusPointNormalY;
    float movement = 0;
    private bool doubleJumpActive = false;
    private float jump;
    private void OnEnable() {
        jumpAction.action.started += (_) => {Jump();};
        jumpAction.action.canceled += (_) => {JumpCancel();};
        belowCheckAction.action.started += (_) => {CheckBelowStart();};
        belowCheckAction.action.canceled += (_) => {CheckBelowCancel();};

    }

    private void OnDisable() {
        jumpAction.action.started -= (_) => {Jump();};
        jumpAction.action.canceled -= (_) => {JumpCancel();};
        belowCheckAction.action.started -= (_) => {CheckBelowStart();};
        belowCheckAction.action.canceled -= (_) => {CheckBelowCancel();};
    }

    void Start()
    {
        focusPointNormalY = focusPoint.localPosition.y;
    }

    void Update()
    {
        
    }

    void Jump()
    {
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
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, checkBelowPoint, focusPoint.localPosition.z);
    }

    void CheckBelowCancel()
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
        if(walkDir < 0 && !spriteRenderer.flipX) Flip();
        else if(walkDir > 0 && spriteRenderer.flipX) Flip();
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
                CheckBelowCancel();
            }
        }

        if(IsGrounded())
        {
            playerAnimator.SetInteger("velocityY", 0);
            airTime = 0;
            body.velocity = new Vector2(movement, body.velocity.y);
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
       
        body.AddForce(new Vector2(0,jump));

    }

    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        focusPoint.localPosition = new Vector3(-focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
    }


}
