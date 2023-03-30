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
    [SerializeField] InputActionReference walkAction, jumpAction, belowCheckAction, aboveCheckAction, rightCheckAction, leftCheckAction;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform focusPoint;
    [SerializeField] Vector2 checkPoints;
    [SerializeField] CapsuleCollider2D playerCollider;
    //[SerializeField] Sprite normalSprite;
    //[SerializeField] Sprite jumpSprite;
    //[SerializeField] Sprite fallSprite;
    private float airTime;

    public MovementRoot movementRoot = new MovementRoot(new string[0]);
    private Vector2 focusPointNormal;
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
        rightCheckAction.action.started += (_) => {CheckRightStart();};
        rightCheckAction.action.canceled += (_) => {CheckCancel();};
        leftCheckAction.action.started += (_) => {CheckLeftStart();};
        leftCheckAction.action.canceled += (_) => {CheckCancel();};
    }

    private void OnDisable() {
        jumpAction.action.started -= (_) => {Jump();};
        jumpAction.action.canceled -= (_) => {JumpCancel();};
        belowCheckAction.action.started -= (_) => {CheckBelowStart();};
        belowCheckAction.action.canceled -= (_) => {CheckCancel();};
        aboveCheckAction.action.started -= (_) => {CheckAboveStart();};
        aboveCheckAction.action.canceled -= (_) => {CheckCancel();};
        rightCheckAction.action.started -= (_) => {CheckRightStart();};
        rightCheckAction.action.canceled -= (_) => {CheckCancel();};
        leftCheckAction.action.started -= (_) => {CheckLeftStart();};
        leftCheckAction.action.canceled -= (_) => {CheckCancel();};
    }

    void Start()
    {
        focusPointNormal = focusPoint.localPosition;
    }

    void Update()
    {
        
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
        movementRoot.SetRoot("CameraCheck", true);
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, -checkPoints.y, focusPoint.localPosition.z);
    }

    void CheckAboveStart()
    {
        movementRoot.SetRoot("CameraCheck", true);
        focusPoint.localPosition = new Vector3(focusPoint.localPosition.x, checkPoints.y, focusPoint.localPosition.z);
    }

    void CheckRightStart()
    {
        movementRoot.SetRoot("CameraCheck", true);
        focusPoint.localPosition = new Vector3(checkPoints.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
    }

    void CheckLeftStart()
    {
        movementRoot.SetRoot("CameraCheck", true);
        focusPoint.localPosition = new Vector3(checkPoints.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
        Flip(false);
    }

    void CheckCancel()
    {
        focusPoint.localPosition = new Vector3(((focusPoint.localPosition.x<0)?-1:1) * focusPointNormal.x, focusPointNormal.y, focusPoint.localPosition.z);
        movementRoot.SetRoot("CameraCheck", false);
        Flip(true);
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
        if(walkDir < 0 && !spriteRenderer.flipX) Flip(false);
        else if(walkDir > 0 && spriteRenderer.flipX) Flip(true);
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

    private void Flip(bool right)
    {
        spriteRenderer.flipX = !right;
        focusPoint.localPosition = new Vector3((right)?1:-1 *  focusPoint.localPosition.x, focusPoint.localPosition.y, focusPoint.localPosition.z);
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
