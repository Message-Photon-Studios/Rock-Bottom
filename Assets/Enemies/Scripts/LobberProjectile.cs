using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class LobberProjectile : Enemy
{
    [SerializeField] int damage;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float startIdleTime;
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float turnSpeed;
    [SerializeField] Quaternion startRotation;
    [SerializeField] ParticleSystem aim;
    [SerializeField] ParticleSystem rocksFalling;

    protected override Node SetupTree()
    {
        Node root =
        new Selector(new List<Node>{

            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, attackTrigger),
                new DamagePlayer(stats, player, damage),
                new SuicideEnemy(stats)
            }),

            new Sequence(new List<Node>{
                new CheckBool("startStop", true),
                new Wait(0.07f),
                new EnemySetVelocity(GetComponent<Rigidbody2D>(), 0),
                new SetParentVariable("startStop", false, 2)
            }),

            new Sequence(new List<Node> {
                new CheckBool("startIdle", true),
                new Wait(startIdleTime+Random.Range(0f, 1f)),
                new SetParentVariable("startIdle", false, 2),
                new ParticlesPlay(rocksFalling, false)
            }),

            new Sequence(new List<Node>{
                new CheckBool("aim", true),
                new HomTowardsTarget(stats, startRotation, player.transform, 1f, turnSpeed, 2f),
                new CheckBool("startIdle", false),
                new ChangeSpeed(stats, maxSpeed, acceleration),
                new Sequence(new List<Node>{
                    new Inverter(new CheckSpeed(stats, maxSpeed/2, maxSpeed/2)),
                    new ParticlesPlay(aim, false),
                    new SetParentVariable("aim", false, 3)
                })
            })
        });

        root.SetData("startIdle",  true);
        root.SetData("aim", true);
        root.SetData("startStop", true);
        
        if(Random.Range(0,10) == 0) animator.SetTrigger("boulder2");

        body?.AddTorque(Random.Range(-20f,20f));
        triggersToFlip.Add(attackTrigger);

        return root;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }
    #endif
}
