using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

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
            new NormalAttack("swordAttack", player, swordDamage, swordForce, attackTrigger, stats),
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackTrigger),
                new PlayAttack(animator, "attack")
                }),
            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .6f, "attack", "walk")
            });
        
        root.SetData("attack", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }
}
