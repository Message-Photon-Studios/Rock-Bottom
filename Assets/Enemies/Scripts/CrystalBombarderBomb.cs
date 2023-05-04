using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalBombarderBomb : Enemy
{
    [SerializeField] float damage;
    [SerializeField] Vector2 force;
    [SerializeField] Trigger attackTrigger;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            
            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new CheckPlayerArea(stats, player, attackTrigger),
                new DamagePlayer(player, damage),
                new AddForcePlayer(stats, player, force),
                new SetParentVariable("attack", false, 2)
            }),

            new Sequence(new List<Node>{
                new EnemyCollide(GetComponent<ColliderCheck>(), ""),
                new AnimationTrigger(animator, "Explode")
            }),

            new Sequence(new List<Node>{
                new LookAtPlayer(stats, player),
                new RunForward(stats, 1f)
            })
            });
        
        root.SetData("attack", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
    }
#endif
}
