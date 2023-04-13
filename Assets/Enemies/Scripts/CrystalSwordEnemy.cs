using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

/// <summary>
/// This is the ai for the Crystoffer enemy
/// </summary>
public class CrystalSwordEnemy : Enemy
{   
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float swordDamage;
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
            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .4f, "attack", "walk")
            });
        
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    /*
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }*/
}
