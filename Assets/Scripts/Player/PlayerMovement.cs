using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] InputActionReference walkAction, jumpAction;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform focusPoing;

    private float jump;
    private void OnEnable() {
        jumpAction.action.performed += (_) => {Jump();};
    }

    private void OnDisable() {
        jumpAction.action.performed -= (_) => {Jump();};
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    void Jump()
    {
        jump = jumpHeight;
    }

    private void FixedUpdate() {
        float walkDir = walkAction.action.ReadValue<float>();
        if(walkDir < 0 && !spriteRenderer.flipX) Flip();
        else if(walkDir > 0 && spriteRenderer.flipX) Flip();
        float movement = movementSpeed * walkDir;
        Vector2 velocity = new Vector2(movement, jump);
        jump = 0;
        body.AddForce(velocity);
    }

    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        focusPoing.localPosition = new Vector3(-focusPoing.localPosition.x, focusPoing.localPosition.y, focusPoing.localPosition.z);
    }

    void Test(int test)
    {

    }


}
