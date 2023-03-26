using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpFalloff;
    [SerializeField] InputActionReference walkAction, jumpAction, belowCheckAction;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform focusPoint;
    [SerializeField] float checkBelowPoint;
    [SerializeField] CapsuleCollider2D collider;


    private float airTime;

    private float focusPointNormalY;
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
            Debug.Log("Grounded");
            jump = jumpHeight;
            doubleJumpActive = false;
            return;
        }

        if(!doubleJumpActive)
        {
            doubleJumpActive = true;
            jump = jumpHeight;
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
        return  Physics2D.Raycast(transform.position+Vector3.right* collider.size.x/2, Vector2.down, 1f, 3) ||
                Physics2D.Raycast(transform.position-Vector3.right* collider.size.x/2, Vector2.down, 1f, 3);
    }

    private void FixedUpdate() {
        float walkDir = walkAction.action.ReadValue<float>();
        if(walkDir < 0 && !spriteRenderer.flipX) Flip();
        else if(walkDir > 0 && spriteRenderer.flipX) Flip();
        float movement = movementSpeed * walkDir;
        Vector2 velocity = new Vector2(movement, jump);
        if(jump > 0)
            jump -= jumpFalloff * Time.fixedDeltaTime;
        else if(jump < 0)
            jump = 0;

        body.AddForce(velocity);

        if(airTime > 1f)
        {
            CheckBelowStart();
            if(IsGrounded())
            {
                CheckBelowCancel();
            }
        }

        if(IsGrounded())
        {
            airTime = 0;
            if(doubleJumpActive) doubleJumpActive = false;
    
        } else
        {
            airTime += Time.fixedDeltaTime;
        }

    }

    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        focusPoint.localPosition = new Vector3(-focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
    }


}
