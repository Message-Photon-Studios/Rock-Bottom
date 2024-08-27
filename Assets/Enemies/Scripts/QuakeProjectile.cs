using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class QuakeProjectile : Enemy
{
    [SerializeField] float startIdleTime;
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float turnSpeed;
    [SerializeField] Quaternion startRotation;

    protected override Node SetupTree()
    {
        Node root =
        new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new EnemyCollide(GetComponent<ColliderCheck>(), "Player"),
                new ChangeCollisionDetection(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false)
            }),

            new Sequence(new List<Node> {
                new HomTowardsPlayer(stats, startRotation, player, 1f, turnSpeed, 2f),
                new CheckBool("startIdle", true),
                new Wait(startIdleTime+Random.Range(0f, 1f)),
                new SetParentVariable("startIdle", false, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("startIdle", false),
                new ChangeSpeed(stats, maxSpeed, acceleration)
            })
        });

        root.SetData("startIdle",  true);

        return root;
    }
}
