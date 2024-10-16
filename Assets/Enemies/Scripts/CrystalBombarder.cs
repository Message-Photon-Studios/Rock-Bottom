using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalBombarder : Enemy
{
    [SerializeField] Trigger attackTrigger;
    [SerializeField] GameObject attackSpawn;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] Vector2 spawnBombForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;
    [SerializeField] float attackDelay;

    protected override Node SetupTree()
    {
        
        Node root = new Sequence(new List<Node>{

            new KeepHeight(stats, transform.position.y, 1f),
            
            new Selector(new List<Node>{
                new Sequence(new List<Node>{
                    new CheckBool("spawnAttack", true),
                    new EnemyObjectSpawner(stats, attackSpawn, spawnOffset, spawnBombForce, true),
                    new SetParentVariable("spawnAttack", false, 3)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("canAttack", true),
                    new CheckBool("attack", false),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new LookAtPlayer(stats, player),
                    new AnimationTrigger(animator, "attack"),
                    new SetParentVariable("canAttack", false, 3)
                    }),

                new Sequence(new List<Node>{
                    new CheckBool("canAttack", false),
                    new Wait(attackDelay),
                    new SetParentVariable("canAttack", true, 3)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("attack", false),
                    new AirPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .7f, "spawnAttack", "move")
                    })
                })
            });
        
        root.SetData("attack", false);
        root.SetData("spawnAttack", false);
        root.SetData("canAttack", true);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
        Handles.DrawSolidDisc(transform.position + (Vector3)spawnOffset, Vector3.forward, .05f);
    }
#endif
}
