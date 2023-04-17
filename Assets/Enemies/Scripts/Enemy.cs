using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BehaviourTree;

/// <summary>
/// This class is an abstract class extended by all enemy AIs. It haves important general functionallity and variables used by all enemy AI. 
/// </summary>
[RequireComponent(typeof(EnemyStats), typeof(SpriteRenderer), typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : BehaviourTree.Tree
{
    [SerializeField] float playerCollisionDamage = 10; //The damage that will be dealt to the player if they walk into the enemy
    [SerializeField] Vector2 playerCollisionForce = new Vector2(2000, 0.5f); //The force that will be added to the player if they walk into the enemy
    protected EnemyStats stats;
    protected Animator animator;
    protected Rigidbody2D body; 
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    protected PlayerStats player;

    /// <summary>
    /// Add triggers to this list if tou want them to be flipped in sync with the enemy
    /// </summary>
    /// <typeparam name="Trigger"></typeparam>
    /// <returns></returns>
    protected List<Trigger> triggersToFlip = new List<Trigger>();

    #region Setup and Updates
    private void OnEnable()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }
    void OnValidate()
    {
        stats = GetComponent<EnemyStats>();
        myCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if(Mathf.Abs(body.velocity.x) > .1f && ((body.velocity.x < 0) != (!spriteRenderer.flipX))) SwitchDirection();
    }

    #endregion

    #region  Collision with player
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            other.rigidbody.AddForce(Vector2.up * playerCollisionForce.y + ((Vector2)player.transform.position-stats.GetPosition()) * Vector2.right * playerCollisionForce.x);
            body.velocity = new Vector2(0, body.velocity.y);
            other.gameObject.GetComponent<PlayerStats>().DamagePlayer(playerCollisionDamage);
            other.gameObject.GetComponent<PlayerMovement>().movementRoot.SetRoot(gameObject.name + "enemyCollision", 0.35f);
        }
    }
    #endregion
    
    #region Switches the players direction
    protected void SwitchDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        myCollider.offset = new Vector2(-myCollider.offset.x, myCollider.offset.y);
        stats.lookDir = -stats.lookDir;
        foreach (Trigger trigger in triggersToFlip)
        {
            trigger.Flip();
        }
    }
    #endregion
    
    #region Data setters

    /// <summary>
    /// Sets a bool in the behaviour tree root to true.
    /// </summary>
    /// <param name="name"></param>
    public void SetBoolTrue(string name)
    {
        root.SetData(name, true);
    }

    /// <summary>
    /// Sets a bool in the behaviour tree root to false
    /// </summary>
    /// <param name="name"></param>
    public void SetBoolFalse(string name)
    {
        root.SetData(name, false);
    }

    #endregion
}

#region Trigger
/// <summary>
/// A small class that handles triggers
/// </summary>
[System.Serializable]
public class Trigger
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
#endregion
