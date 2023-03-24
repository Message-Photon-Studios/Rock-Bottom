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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Gizmos.DrawSphere(pos, 1);
    }

    void Jump()
    {
        jump = jumpHeight;
    }

    private void FixedUpdate() {
        float movement = movementSpeed * walkAction.action.ReadValue<float>();
        Vector2 velocity = new Vector2(movement, jump);
        jump = 0;
        body.AddForce(velocity);
    }

    void Test(int test)
    {

    }


}
