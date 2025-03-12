using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class BigBettan : Enemy
{
    [SerializeField] float viewRange;
    [SerializeField] Trigger attackFrontFirstTrigger;
    [SerializeField] Trigger attackFrontTrigger;
    [SerializeField] Trigger attackBackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float patrollIdleTime;
    float legPos = .8f;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new Selector(new List<Node>
                {
                    new NormalAttack("swordFrontAttackFirst", player, swordDamage, swordForce, 0.5f, attackFrontFirstTrigger, stats),
                    new NormalAttack("swordFrontAttack", player, swordDamage, swordForce, 0.5f, attackFrontTrigger, stats),
                    new NormalAttack("swordBackAttack", player, swordDamage, swordForce, 0.5f, attackBackTrigger, stats)
                }),
                new SetParentVariable("attackDone", true, 2)
            }),
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackFrontFirstTrigger),
                new AnimationTrigger(animator, "attack")
                }),

            new Sequence(new List<Node>{
                new PlatformChase(stats, player, body, animator, 1f, viewRange, -.5f, legPos ,"attack", "walk")
            }),

            new RandomPatroll(stats, body, animator, 1, patrollIdleTime, legPos, "attack", "walk")

            });
        
        root.SetData("swordFrontAttackFirst", false);
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordFrontAttack", false);
        root.SetData("swordBackAttack", false);
        triggersToFlip.Add(attackFrontFirstTrigger);
        triggersToFlip.Add(attackFrontTrigger);
        triggersToFlip.Add(attackBackTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackFrontTrigger.DrawTrigger(stats.GetPosition());
        attackBackTrigger.DrawTrigger(stats.GetPosition());
        attackFrontFirstTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.blue;
        Handles.DrawLine(stats.GetPosition()+Vector2.left*viewRange, stats.GetPosition()+Vector2.right*viewRange);
    }
#endif
}
