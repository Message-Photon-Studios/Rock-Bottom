using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandHunter : Enemy
{
    [SerializeField] Trigger attackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float timeBetweenAttack;
    [SerializeField] Quaternion startRotation;
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>{

            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("swordAttack", player, swordDamage, swordForce, 0.5f, attackTrigger, stats),
                new SetParentVariable("attackDone", true, 2),
            }),

            new Selector(new List<Node>{
                new Sequence(new List<Node>{
                    new CheckBool("attack", false),
                    new CheckBool("idle", false),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new AnimationTrigger(animator, "attack"),
                    new SetParentVariable("idle", true, 3)
                    }),

                new Sequence(new List<Node>{
                    new CheckBool("idle", false),
                    new LookAtPlayer(stats, player),
                    new HomTowardsPlayer(stats, startRotation, player, 1f, 1000, 3f),
                    new AnimationTrigger(animator, "walk")
                })
            }),
            new Sequence(new List<Node>{
                new Wait(timeBetweenAttack),
                new SetParentVariable("idle", false, 2)
            }),
            });

        root.SetData("idle", false);
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        root.SetData("castSpell", false);
        root.SetData("spellMode", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    private void OnDestroy()
    {
        GameManager.instance.RespawnHunter();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }
#endif
}
