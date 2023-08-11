using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalQuake : Enemy
{
    [SerializeField] Trigger viewTrigger;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    private float legPos = 1.8f;

    protected override Node SetupTree()
    {

        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("swordAttack", player, swordDamage, swordForce, 0.5f, attackTrigger, stats),
                new SetParentVariable("attackDone", true, 2)
            }),
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackTrigger),
                new AnimationTrigger(animator, "attack")
                }),

            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, viewTrigger),
                new PlatformChase(stats, player.transform, body, animator, 1f, legPos ,"attack", "walk")
            }),

            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, legPos, "attack", "walk")

            });

        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(viewTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        attackTrigger.DrawTrigger(stats.GetPosition());
        viewTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left * patrollDistance, stats.GetPosition() + Vector2.right * patrollDistance);
    }
#endif
}
