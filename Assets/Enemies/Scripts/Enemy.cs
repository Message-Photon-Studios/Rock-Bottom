using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BehaviourTree;

[RequireComponent(typeof(EnemyStats), typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class Enemy : BehaviourTree.Tree
{
    [SerializeField] float playerCollisionDamage = 10;
    [SerializeField] float playerCollisionForce = 2000;
    private float rootTimer = 0;
    protected EnemyStats stats;
    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    protected PlayerStats player;
    private void OnEnable()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }
    void OnValidate()
    {
        stats = GetComponent<EnemyStats>();
        myCollider = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        base.Update();
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
            other.rigidbody.AddForce(((Vector2)other.transform.position + Vector2.up*0.5f - stats.GetPosition()) * playerCollisionForce);
            other.gameObject.GetComponent<PlayerStats>().DamagePlayer(playerCollisionDamage);
            other.gameObject.GetComponent<PlayerMovement>().movementRoot.SetRoot("enemyCollision", true);
            rootTimer = 0.5f;
        }
    }
    protected void SwitchDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        myCollider.offset = new Vector2(-myCollider.offset.x, myCollider.offset.y);
    }

    public void SetBoolTrue(string name)
    {
        root.SetData(name, true);
    }

    public void SetBoolFalse(string name)
    {
        root.SetData(name, false);
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
