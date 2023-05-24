using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using Sequence = BehaviourTree.Sequence;

public class Spikey : Enemy
{
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger damageTrigger;
    [SerializeField] int dropDamage;
    [SerializeField] Vector2 dropPlayerForce;
    [SerializeField] float sideDistTrigger;
    [SerializeField] float heightDistTrigger;
    [SerializeField] float droppHeight;
    [SerializeField] float dropIdleTime;
    [SerializeField] float hoverSpeedFactor;
    [SerializeField] float patrollSpeedFactor;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    [Space(10)]
    [SerializeField] ParticleSystem rocks;

    protected override Node SetupTree()
    {
        
        Node root =  new Selector(new List<Node>{
                
                new Sequence(new List<Node>{
                    new CheckBool("alert", false),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new AnimationBool(animator, "alert", true),
                    new SetParentVariable("alert", true, 2)
                }),


                new Sequence(new List<Node>{
                    new CheckBool("alert", true),
                    new CheckBool("attack", false),
                    new CheckPlayerDirection(stats, player, Vector2.right, -sideDistTrigger, sideDistTrigger),
                    new CheckPlayerDirection(stats, player, Vector2.down, heightDistTrigger, droppHeight),
                    new SetParentVariable("attack", true, 2)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", true),
                    new Wait(dropIdleTime),
                    new SetParentVariable("attack", false, 2),
                    new AnimationBool(animator, "stuck", false)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", true),
                    new CheckGrounded(stats, .2f),
                    new AnimationBool(animator, "stuck", true)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", true),
                    new CheckPlayerArea(stats, player, damageTrigger),
                    new DamagePlayer(player, dropDamage),
                    new AddForcePlayer(stats, player, dropPlayerForce)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", true),
                    new Inverter(new CheckGrounded(stats, .5f)),
                    new KeepHeight(stats, stats.GetPosition().y-droppHeight, 1f),
                    }),

                new Sequence(new List<Node>{
                    new CheckBool("alert", true),
                    new Inverter(new CheckPlayerArea(stats, player, attackTrigger)),
                    new SetParentVariable("alert", false, 2),
                    new AnimationBool(animator, "alert", false),
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", false),
                    new KeepHeight(stats, stats.GetPosition().y, hoverSpeedFactor),
                    new AirPatroll(stats, body, animator, patrollDistance, patrollSpeedFactor, patrollIdleTime, .7f, "spawnAttack", "move")
                })
            });
        
        root.SetData("alert", false);
        root.SetData("attack", false);
        root.SetData("spawnAttack", false);
        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(damageTrigger);
        return root;
    }


    protected override void Update()
    {
        base.Update();
        animator.SetInteger("velocityY", Mathf.RoundToInt(body.velocity.y));
    }

    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        damageTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
        Handles.color = Color.red;
        Handles.DrawLine(stats.GetPosition() +  Vector2.down * heightDistTrigger, stats.GetPosition() + Vector2.down * droppHeight);
        Handles.DrawLine(stats.GetPosition() + Vector2.left * sideDistTrigger, stats.GetPosition()+Vector2.right*sideDistTrigger);
    }
#endif
}
