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
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger attackDamageTrigger;
    [SerializeField] int clubDamage;
    [SerializeField] float clubForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

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
                new CheckPlatformEdgePartly(stats, 1.2f),
                new AnimationBool(animator, "run", false),
                new AnimationTrigger(animator, "attack"),
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
                new AnimationBool(animator, "run", true),
                new SetParentVariable("chase", true, 2),
                new PlatformChase(stats, player, body, animator, runSpeedFactor, viewRange, 0f, .5f ,"attack", "run")
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
                new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")
            }),
            

            });
        
        root.SetData("chaseCooldown", false);
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
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
