using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalCrawler : Enemy
{
    [SerializeField] Trigger viewTrigger;
    [SerializeField] Trigger damageTrigger;
    [SerializeField] float attackDamage;
    [SerializeField] float attackForce;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float forwardJumpForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    private float legPos = .5f;
    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("prusuit", true),

                new Selector(new List<Node>{

                    new Sequence(new List<Node>{
                        new CheckBool("enableJump", true),
                        new CheckGrounded(stats, legPos),
                        new Inverter(new CheckPlayerArea(stats, player, damageTrigger)),
                        new Selector(new List<Node>{
                            new CheckPlayerDirection(stats,player,Vector2.up, 0.5f, 200f),
                        }),
                        new Inverter(new CheckPlayerBehind(stats, player)),
                        new Inverter(new CheckRoof(stats)),
                        new EnemyJump(stats, body, jumpForce, forwardJumpForce),
                        new SetParentVariable("enableJump", false, 4),
                        new SetParentVariable("enableDamage", true, 4)
                    }),

                    new Sequence(new List<Node>{
                        new CheckBool("attackDone", false),
                        new NormalAttack("enableDamage", player, attackDamage, attackForce, 0.5f, damageTrigger, stats),
                        new SetParentVariable("attackDone", true, 4)
                    }),

                    new Sequence(new List<Node>{
                        new CheckBool("enableJump", false),
                        new Wait(1),
                        new SetParentVariable("enableJump", true, 4)
                    }),

                    new Sequence(new List<Node>{
                        new Selector(new List<Node>{
                            new CheckPlayerDistance(stats, player, 10, 20),
                            new CheckPlayerArea(stats, player, viewTrigger),
                        }),
        
                        new CheckGrounded(stats, legPos),
                        new SetParentVariable("attackDone", false, 4),
                        new SetParentVariable("enableDamage", false, 4),
                        new AnimationBool(animator, "move", true),
                        new PlatformChase(stats, player.transform, body, animator, runSpeed, legPos ,"attack", "run"),
                    }),

                    new Sequence(new List<Node>{
                        new CheckPlayerDistance(stats,player,20, 2000),
                        new SetParentVariable("prusuit", false, 4)
                    }),

                    new Sequence(new List<Node>{
                        new CheckWall(stats,Vector2.right, 1f),
                        new CheckGrounded(stats,legPos),
                        new EnemyJump(stats, body, jumpForce, forwardJumpForce),
                        new SetParentVariable("enableJump", false, 4),
                        new SetParentVariable("enableDamage", true, 4)
                    }),

                    new Sequence(new List<Node>{
                        new RunForward(stats, runSpeed),
                        new AnimationBool(animator, "run", true)
                    })
                })
            }),
     

            new Sequence(new List<Node>{ //If the enemy can see the player then change state to prusuit
                new CheckPlayerArea(stats, player, viewTrigger),
                new SetParentVariable("prusuit", true, 2)
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
        root.SetData("prusuit", false);

        triggersToFlip.Add(viewTrigger);
        triggersToFlip.Add(damageTrigger);
        return root;
    }

    protected override void Update()
    {
        base.Update();
        animator.SetInteger("velocityY", (int)body.velocity.y);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        viewTrigger.DrawTrigger(stats.GetPosition());
        damageTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
