using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalCrawler : Enemy
{
    [SerializeField] Trigger viewTrigger;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger attackBottomTrigger;
    [SerializeField] Trigger damageTrigger;
    [SerializeField] float attackDamage;
    [SerializeField] float attackForce;
    [SerializeField] float jumpForce;
    [SerializeField] float forwardJumpForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    private float legPos = .8f;
    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, damageTrigger),
                new SetParentVariable("enableJump", false, 2)
            }),
            
            new Sequence(new List<Node>{
                new CheckBool("enableJump", true),
                new CheckGrounded(stats, legPos),
                new Selector(new List<Node>{
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new CheckPlayerArea(stats, player, attackBottomTrigger)
                }),
                new EnemyJump(stats, body, jumpForce, forwardJumpForce),
                new SetParentVariable("enableJump", false, 2),
                new SetParentVariable("enableDamage", true, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("enableDamage", player, attackDamage, attackForce, 0.5f, damageTrigger, stats),
                new SetParentVariable("attackDone", true, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("enableJump", false),
                new Wait(2),
                new SetParentVariable("enableJump", true, 2)
            }),

            new Sequence(new List<Node>{
                new CheckGrounded(stats, legPos),
                new SetParentVariable("attackDone", false, 2),
                new SetParentVariable("enableDamage", false, 2),
                new CheckPlayerArea(stats, player, viewTrigger),
                new AnimationBool(animator, "move", true),
                new PlatformChase(stats, player.transform, body, animator, 2.2f, legPos ,"attack", "run"),
            }),
            
            new Sequence(new List<Node>{
                new CheckGrounded(stats,legPos),
                new AnimationBool(animator, "run", false),
                new RandomPatroll(stats, body, animator, patrollDistance, 1f, patrollIdleTime, legPos, "attack", "move")
            })
        });
        
        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("swordAttack", false);
        root.SetData("enableJump", true);

        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(viewTrigger);
        triggersToFlip.Add(damageTrigger);
        triggersToFlip.Add(attackBottomTrigger);
        return root;
    }

    protected override void Update()
    {
        base.Update();
        animator.SetInteger("velocityY", (int)body.velocity.y);
    }

    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        viewTrigger.DrawTrigger(stats.GetPosition());
        damageTrigger.DrawTrigger(stats.GetPosition());
        attackBottomTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
}
