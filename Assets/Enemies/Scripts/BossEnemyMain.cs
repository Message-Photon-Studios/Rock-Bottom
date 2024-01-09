using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class BossEnemyMain : Enemy
{
    [SerializeField] float wispTimer;
    [SerializeField] GameObject wispTmp;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] Vector2 spawnForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Sequence(new List<Node>{

            new KeepHeight(stats, transform.position.y, 1f),

            new Selector(new List<Node>{

                new Sequence(new List<Node>{
                    new Wait(wispTimer),
                    new EnemyObjectSpawner(stats, wispTmp, spawnOffset, spawnForce)
                }),

                new AirPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .7f, "attack", "move")
            })
        });
        
        root.SetData("activateBeam", false);
        root.SetData("attack", false);
        root.SetData("sleeping", false);
        return root;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
