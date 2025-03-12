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
    //[SerializeField] int playerCollisionDamage = 10; //The damage that will be dealt to the player if they walk into the enemy
    //[SerializeField] Vector2 playerCollisionForce = new Vector2(2000, 0.5f); //The force that will be added to the player if they walk into the enemy
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
        stats.onDamageTaken += DamageTaken;
    }

    protected virtual void OnDisable()
    {
        stats.onDamageTaken -= DamageTaken;
    }
    void OnValidate()
    {
        stats = GetComponent<EnemyStats>();
        myCollider = GetComponent<Collider2D>();
    }

    protected virtual void DamageTaken(float damage, Vector2 atPostion)
    {}
    protected virtual void Update()
    {
        /*if(body != null)
            if(Mathf.Abs(body.velocity.x) > .1f && ((body.velocity.x < 0) != (!spriteRenderer.flipX))) SwitchDirection();*/
    }

    public virtual void DamagePlayer(){}

    #endregion

    /*

    #region  Collision with player
    public void CollideWithPlayer(GameObject player)
    {
        if(player.CompareTag("Player") && !stats.IsAsleep())
        {
            player.gameObject.GetComponent<Rigidbody2D>().AddForce(((Vector2)player.transform.position-stats.GetPosition()) * Vector2.right * playerCollisionForce.x);
            body.velocity = new Vector2(0, body.velocity.y);
            player.gameObject.GetComponent<PlayerStats>().DamagePlayer(stats.GetScaledDamage(playerCollisionDamage), stats);
            player.gameObject.GetComponent<PlayerMovement>().movementRoot.SetRoot(gameObject.name + "enemyCollision", 0.3f);
        }
    }
    #endregion

    */
    
    #region Switches the players direction
    public void SwitchDirection()
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

    public void SetVarData(string varName, object value)
    {
        root.SetData(varName, value);
    }

    #endregion
}

#region Trigger
/// <summary>
/// A small class that handles triggers
/// </summary>

[System.Serializable]
public class Trigger {

    [SerializeField] public float radius;
    [SerializeField] public float direction;
    [SerializeField] public float width = 360;
    [SerializeField] public Vector2 offset;
    [SerializeField] private Color color;

    public void Flip()
    {
        offset = new Vector2(-offset.x, offset.y);
        direction = ((-(direction-90))+90)%360;
    }

    public void RotateRight()
    {
        direction = (direction - 90) % 360;
        float lengh = Mathf.Sqrt(Mathf.Pow(offset.x,2) + Mathf.Pow(offset.y,2));
        if (lengh == 0) return;
        float rad = Mathf.Atan2(offset.y, offset.x);
        offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad))*lengh;
    }

#if UNITY_EDITOR
    public void DrawTrigger(Vector2 position)
    {
        Handles.color = color;
        Handles.DrawWireDisc(position + offset, Vector3.forward, radius);
        Handles.DrawLine(position + offset, new Vector2(Mathf.Cos((direction + (width / 2)) * Mathf.Deg2Rad)*radius + position.x,Mathf.Sin((direction + (width / 2))*Mathf.Deg2Rad)*radius+position.y) + offset);
        Handles.DrawLine(position + offset, new Vector2(Mathf.Cos((direction - (width / 2)) * Mathf.Deg2Rad) * radius + position.x, Mathf.Sin((direction - (width / 2)) * Mathf.Deg2Rad) * radius + position.y) + offset);
    }
#endif
}
#endregion
