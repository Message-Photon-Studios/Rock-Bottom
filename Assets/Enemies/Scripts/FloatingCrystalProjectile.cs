using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class FloatingCrystalProjectile : Enemy
{
    [SerializeField] int damage;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float rotationSpeed;
    [SerializeField] float randomFollowFactor;
    Quaternion startRotation;

    protected override Node SetupTree()
    {
        startRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 359f)));
        transform.rotation = startRotation;
        Node root = new Selector(new List<Node> 
        {
            new Sequence(new List<Node>{
                new Wait(1f),
                new SuicideEnemy(stats)
            }),

            
            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, attackTrigger),
                new DamagePlayer(stats, player, damage),
                new SuicideEnemy(stats)
            }),

            new Sequence(new List<Node>{
                new EnemyCollide(GetComponent<ColliderCheck>(), "Player"),
                new SuicideEnemy(stats)
            }),

            new Sequence(new List<Node>{
                new HomTowardsTarget(stats, startRotation, player.transform, 1f, rotationSpeed, randomFollowFactor)
            })
        });

        triggersToFlip.Add(attackTrigger);
        return root;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }
    #endif
}
