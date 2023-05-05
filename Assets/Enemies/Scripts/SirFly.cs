using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

/// <summary>
/// This is the ai for the Crystoffer enemy
/// </summary>
public class SirFly : Enemy
{

    [SerializeField] Trigger rangeTrigger;
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
                new CheckBool("inRange", false),
                new CheckPlayerArea(stats, player, rangeTrigger),
                new SetParentVariable("inRange", true, 2),
                new AnimationBool(animator, "inRange",true)
                }),
            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .4f, "inRange", "walk")
            });
        
        root.SetData("inRange", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        rangeTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
