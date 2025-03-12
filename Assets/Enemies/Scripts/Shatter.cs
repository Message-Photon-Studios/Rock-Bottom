using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class Shatter : Enemy
{
    [SerializeField] int damage;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger lookTrigger;

    protected override Node SetupTree()
    {

        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new CheckPlayerArea(stats, player, attackTrigger),
                new DamagePlayer(stats, player, damage)
            }),
            
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, lookTrigger),
                new AnimationBool(animator, "attacking", true)
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new Inverter(new CheckPlayerArea(stats, player, lookTrigger)),
                new AnimationBool(animator, "attacking", false)
            })
        });

        root.SetData("attack", false);
        return root;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        lookTrigger.DrawTrigger(stats.GetPosition());
    }
    #endif
}
