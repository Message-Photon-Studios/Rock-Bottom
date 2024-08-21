using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class BigBettan : Enemy
{
    [SerializeField] Trigger viewTrigger;
    [SerializeField] Trigger attackFrontTrigger;
    [SerializeField] Trigger attackBackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new Selector(new List<Node>
                {
                    new NormalAttack("swordFrontAttack", player, swordDamage, swordForce, 0.5f, attackFrontTrigger, stats),
                    new NormalAttack("swordBackAttack", player, swordDamage, swordForce, 0.5f, attackBackTrigger, stats)
                }),
                new SetParentVariable("attackDone", true, 2)
            }),
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackFrontTrigger),
                new AnimationTrigger(animator, "attack")
                }),

            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, viewTrigger),
                new PlatformChase(stats, player, body, animator, 1f, .5f ,"attack", "walk")
            }),

            new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")

            });
        
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordFrontAttack", false);
        root.SetData("swordBackAttack", false);
        triggersToFlip.Add(attackFrontTrigger);
        triggersToFlip.Add(attackBackTrigger);
        triggersToFlip.Add(viewTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackFrontTrigger.DrawTrigger(stats.GetPosition());
        attackBackTrigger.DrawTrigger(stats.GetPosition());
        viewTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
