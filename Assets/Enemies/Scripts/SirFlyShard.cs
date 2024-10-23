using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class SirFlyShard : Enemy
{
    [SerializeField] float damage;
    [SerializeField] Vector2 force;
    [SerializeField] Trigger attackTrigger;

    protected override Node SetupTree()
    {
        
        Node root =     
            new Selector(new List<Node>{
                new Sequence(new List<Node>{
                    new EnemyCollide(GetComponent<ColliderCheck>(), ""),
                    new SuicideEnemy(stats)
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
