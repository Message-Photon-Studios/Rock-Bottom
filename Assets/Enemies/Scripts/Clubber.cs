using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using UnityEngine.UIElements;

public class Clubber : Enemy
{
    [SerializeField] float viewRange;
    [SerializeField] float chaseCooldown;
    [SerializeField] float runSpeedFactor;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpForwardForce;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger attackDamageTrigger;
    [SerializeField] int clubDamage;
    [SerializeField] float clubForce;
    [SerializeField] float patrollIdleTime;
    private float legPos = 2.5f;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{

            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("clubAttack", player, clubDamage, clubForce, 0.5f, attackDamageTrigger, stats),
                new SetParentVariable("attackDone", true, 2),
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chaseCooldown", false),
                new CheckBool("chase", true),
                new CheckPlayerArea(stats, player, attackTrigger),
                new AnimationBool(animator, "run", false),
                new AnimationTrigger(animator, "attack")
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chase", true),
                new CheckVelocity(body, 0, 5),
                new CheckWall(stats, Vector2.right, legPos+0.5f, -.5f),
                new Wait(.1f, 0.1f),
                new EnemyJump(stats, body, jumpForce*1.5f, jumpForwardForce*1.5f)
            }),
            
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chase", true),
                new CheckPlatformEdgePartly(stats, legPos, 2f),
                new Selector(new List<Node>{
                    new Sequence(new List<Node>{
                        new CheckGrounded(stats, legPos, true),
                        new Inverter(new CheckWall(stats, Vector2.right, legPos+1f, .5f)),
                        new EnemyJump(stats, body, jumpForce, jumpForwardForce)
                    }),

                    new Sequence(new List<Node>{
                        new AnimationBool(animator, "run", false),
                        new AnimationTrigger(animator, "attack")
                    })
                })
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chaseCooldown", false),
                new CheckBool("chase", true),
                new CheckBool("attack", false),
                new RunForward(stats, runSpeedFactor)
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chaseCooldown", false),
                new CheckBool("chase", false),
                new Inverter(new IsSleeping(stats)),
                new SeePlayerHorizontal(stats, player, 0f, viewRange), 
                new AnimationBool(animator, "run", true),
                new SetParentVariable("chase", true, 2),
                new PlatformChase(stats, player, body, animator, runSpeedFactor, viewRange, 0f, legPos ,"attack", "run")
            }),

            new Sequence(new List<Node>{
                new CheckBool("chaseCooldown", true),
                new Wait(chaseCooldown),
                new SetParentVariable("chaseCooldown", false, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckBool("chase", false),
                new AnimationBool(animator, "run", false),
                new RandomPatroll(stats, body, animator, 1, patrollIdleTime, legPos, "attack", "walk")
            }),
            

            });
        
        root.SetData("chaseCooldown", true);
        root.SetData("chase", false);
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("clubAttack", false);
        stats.onEnemySlept += OnSleep;
        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(attackDamageTrigger);
        return root;
    }

    private void OnDestroy() {
        stats.onEnemySlept -= OnSleep;
    }

    private void OnSleep()
    {
        Debug.Log("enemy sleep");
        root.SetData("chase", false);
        animator.SetBool("run", false);
        animator.SetBool("walk", false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackDamageTrigger.DrawTrigger(stats.GetPosition());
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.blue;
        Handles.DrawLine(stats.GetPosition()+Vector2.left*viewRange, stats.GetPosition()+Vector2.right*viewRange);
    }
#endif
}
