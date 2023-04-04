using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EnemyStats), typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float detectionDistance;
    [SerializeField] float disengagementDistance;
    [SerializeField] float attackRange;
    [SerializeField] Vector2 attackRangeOffset;
    [SerializeField] float playerCollisionDamage = 20;
    [SerializeField] float playerCollisionForce = 3000;
    private float rootTimer = 0;
    protected EnemyStats stats;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    protected bool playerDetected = false;
    protected PlayerStats player;
    protected int lookDir {get; private set;} = -1;
    private float _movementDir;
    protected float movementDir 
    {
        get{ return _movementDir; }
        set
        {
            if(_movementDir < 0 != value < 0)
            {
                SwitchDirection();
            }

            _movementDir = value;
        }
    }
    private void Start()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }
    void OnValidate()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(!playerDetected && !stats.IsAsleep() && ((Vector2)player.transform.position -GetPosition()).sqrMagnitude < Mathf.Pow(detectionDistance, 2) &&
            Physics2D.Raycast(GetPosition(), (Vector2)player.transform.position - GetPosition()))
        {
            playerDetected = true;
            PlayerDetected();
        }

        if(playerDetected && !stats.IsAsleep() && ((Vector2)player.transform.position - GetPosition()).sqrMagnitude < Mathf.Pow(disengagementDistance, 2))
        {
            playerDetected = false;
            Disengage();
        }

        if(playerDetected && !stats.IsAsleep() && ((Vector2)player.transform.position + attackRangeOffset - GetPosition()).sqrMagnitude < Mathf.Pow(attackRange, 2) &&
            Physics2D.Raycast(GetPosition() + attackRangeOffset, (Vector2)player.transform.position+ attackRangeOffset - GetPosition()))
        {
            AttackPlayer();
        }

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

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(GetPosition(), transform.forward, detectionDistance);
        Handles.color = Color.green;
        Handles.DrawWireDisc(GetPosition(), transform.forward, disengagementDistance);
        Handles.color = Color.red;
        Handles.DrawWireDisc(GetPosition() + attackRangeOffset, transform.forward, attackRange);
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

    public virtual void PlayerDetected()
    {

    }

    public virtual void Disengage()
    {

    }

    public virtual void AttackPlayer()
    {

    }

    protected Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y) + myCollider.offset;
    }

    private void SwitchDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        lookDir *= -1;
        myCollider.offset = new Vector2(-myCollider.offset.x, myCollider.offset.y);
        attackRangeOffset = new Vector2(-attackRangeOffset.x, attackRangeOffset.y);
    }
}
