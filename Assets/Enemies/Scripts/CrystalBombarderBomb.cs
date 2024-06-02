using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalBombarderBomb : Enemy
{
    [SerializeField] int damage;
    [SerializeField] Vector2 force;
    [SerializeField] float turnSpeed;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger detectTrigger;
    [SerializeField] Quaternion startRotation;
    [SerializeField] ParticleSystem aim;

    protected override Node SetupTree()
    {

        Node root =
            new Selector(new List<Node>{

                new Sequence(new List<Node>{
                    new CheckBool("attack", true),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new DamagePlayer(player, damage),
                    new AddForcePlayer(stats, player, force),
                    new SetParentVariable("attack", false, 2)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("exploding", false),
                    new Selector(new List<Node>{
                        new EnemyCollide(GetComponent<ColliderCheck>(), "Player"),
                        new CheckPlayerArea(stats, player, detectTrigger),
                        new CheckSpeed(stats, 500f, 1000f)
                    }),
                    new AnimationTrigger(animator, "Explode"),
                    new SetParentVariable("exploding", true, 2)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("exploding", false),
                    new LookAtPlayer(stats, player),
                    new Selector(new List<Node>{
                        new Inverter(new CheckSpeed(stats, 1000f, 1000f)),
                         new ParticlesPlay(aim, false)
                    }),
                    new HomTowardsPlayer(stats, startRotation, player, 1f, turnSpeed),
                    new ChangeSpeed(stats, 0f, 750f)
                })
            });
        
        root.SetData("attack", false);
        root.SetData("exploding", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        detectTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
    }
#endif
}
