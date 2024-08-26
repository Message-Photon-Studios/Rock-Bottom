using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class Peblit : Enemy
{
    [SerializeField] float viewRange;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("swordAttack", player, swordDamage, swordForce, 0.5f, attackTrigger, stats),
                new SetParentVariable("attackDone", true, 2)
            }),
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackTrigger),
                new AnimationTrigger(animator, "attack")
                }),

            new Sequence(new List<Node>{
                new PlatformChase(stats, player, body, animator, 1f, viewRange, .5f ,"attack", "walk")
            }),

            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")

            });
        
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.blue;
        Handles.DrawLine(stats.GetPosition()+Vector2.left*viewRange, stats.GetPosition()+Vector2.right*viewRange);
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
