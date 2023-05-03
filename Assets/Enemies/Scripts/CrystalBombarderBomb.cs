using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalBombarderBomb : Enemy
{
    [SerializeField] Trigger attackTrigger;
    [SerializeField] GameObject attackSpawn;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] Vector2 spawnBombForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("spawnAttack", true),
                new EnemyObjectSpawner(stats, attackSpawn, spawnOffset, spawnBombForce),
                new SetParentVariable("spawnAttack", false, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackTrigger),
                new LookAtPlayer(stats, player),
                new AnimationTrigger(animator, "attack")
                }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new AirPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .7f, "attack", "move")
                })
            });
        
        root.SetData("attack", false);
        root.SetData("spawnAttack", false);
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
