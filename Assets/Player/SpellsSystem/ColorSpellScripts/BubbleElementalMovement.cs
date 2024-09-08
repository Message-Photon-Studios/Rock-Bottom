using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleElementalMovement : MonoBehaviour
{
    [SerializeField] float slideSpeed;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D body;

    bool hasTriggered = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {   
            Debug.Log("Slide");
            spriteRenderer.flipX = (other.transform.position.x > transform.position.x);
            body.velocity = (new Vector2(slideSpeed*((spriteRenderer.flipX)?-1:1), 0));
            hasTriggered = true;
        }   
    }

    void Update()
    {
        animator.SetBool("slide", (body.velocity.x != 0));
    }
}
