using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using System;

public class Wisp : Enemy
{
    GameObject target = null;
    protected override Node SetupTree()
    {
        target = player?.gameObject;
        Node root =     
            new Selector(new List<Node>{

                new Sequence(new List<Node>{
                    new CheckTargetDistance(stats, "target", 2f),
                    new SetEnemyColorToMine(stats, "target"),
                    new AnimationTrigger(animator, "dead")
                }),
            
                new Sequence(new List<Node>{
                    new Selector(new List<Node>{
                        new EnemyCollide(GetComponent<ColliderCheck>(), "Player"),
                        new EnemyCollide(GetComponent<ColliderCheck>(), "Enemy"),
                        new CheckGrounded(stats,0.2f)
                    }),
                    new AnimationTrigger(animator, "dead")
                }),

                new Sequence(new List<Node>{
                    new LookAtTarget(stats, "target"),
                    new RunForward(stats, 1f)
                })
            });

        FindObjectOfType<BossEnemyController>().WispSpawned(gameObject);
        
        root.SetData("target", target);
        return root;
    }

    public void SetTarget(GameObject newTarget){
        target = newTarget;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
    }
#endif
}
