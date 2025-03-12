using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class Krystina : Enemy
{
    [SerializeField] int damage;
    [SerializeField] float attackForce;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new NormalAttack("krystinaAttack", player, damage, attackForce, 0.5f, attackTrigger, stats),
                new AnimationBool(animator, "walk", false),
            }),

            new RandomPatroll(stats, body, animator, 1, patrollIdleTime, .4f, "attack", "walk"),
        });
        
        
        root.SetData("attack", false);
        root.SetData("krystinaAttack", true);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }
    #endif
}
