using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EnemyStats), typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float playerCollisionDamage = 10;
    [SerializeField] float playerCollisionForce = 3000;
    private float rootTimer = 0;
    [SerializeField] protected EnemyStats stats;
    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    [SerializeField] protected PlayerStats player;
    private void Start()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }
    void OnValidate()
    {
        myCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if(rootTimer > 0)
        {
            rootTimer -= Time.deltaTime;
            if(rootTimer <= 0)
            {
                rootTimer = 0;
                player.GetComponent<PlayerMovement>().movementRoot.SetRoot("enemyCollision", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            other.rigidbody.AddForce(((Vector2)other.transform.position + Vector2.up*0.5f - GetPosition()) * playerCollisionForce);
            other.gameObject.GetComponent<PlayerStats>().DamagePlayer(playerCollisionDamage);
            other.gameObject.GetComponent<PlayerMovement>().movementRoot.SetRoot("enemyCollision", true);
            rootTimer = 0.5f;
        }
    }

    protected Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y) + myCollider.offset;
    }

    protected void SwitchDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        myCollider.offset = new Vector2(-myCollider.offset.x, myCollider.offset.y);
    }

    protected bool CheckTrigger(Trigger trigger)
    {
        var hit = Physics2D.Raycast(GetPosition() + trigger.offset, (Vector2)player.transform.position - trigger.offset - GetPosition(), trigger.radius);
        return (!stats.IsAsleep() && hit.collider != null && hit.collider.CompareTag("Player"));
    }
}

[System.Serializable]
public struct Trigger
{
    [SerializeField] public float radius;
    [SerializeField] public Vector2 offset;
    [SerializeField] private Color color;

    public void Flip()
    {
        offset = new Vector2(-offset.x, offset.y);
    }
    
    public void DrawTrigger(Vector2 position)
    {
        Handles.color = color;
        Handles.DrawWireDisc(position+offset, Vector3.forward, radius);
    }
}
