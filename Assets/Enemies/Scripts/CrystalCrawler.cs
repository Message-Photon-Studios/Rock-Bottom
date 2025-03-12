using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using UnityEngine.UIElements;
using Steamworks;

public class CrystalCrawler : Enemy
{
    [SerializeField] int damage;
    [SerializeField] float attackForce;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger viewTrigger;
    [SerializeField] Trigger preventJump;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float checkJumpHight;
    [SerializeField] float forwardJumpForce;
    [SerializeField] float smallJumpForce;
    [SerializeField] float smallJumpForward;
    [SerializeField] float jumpIdleTime;
    [SerializeField] float patrollIdleTime;

    private float legPos = .5f;
    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("prusuit", true),

                new Selector(new List<Node>{
                    
                    new Sequence(new List<Node>{
                        new Inverter(new CheckPlayerArea(stats, player, viewTrigger)),
                        new Wait(2f, 1f),
                        new SetParentVariable("prusuit", false, 4)
                    }),

                    new Sequence(new List<Node>{
                        new CheckBool("prusuit", true),
                        new NormalAttack("crawlerAttack", player, damage, attackForce, 0.5f, attackTrigger, stats),
                        new CheckGrounded(stats, legPos),
                        new EnemyJump(stats, body, smallJumpForce, smallJumpForward),
                    }),

                    new Sequence(new List<Node>{
                        new CheckBool("enableJump", true),
                        new CheckGrounded(stats, legPos),
                        new Inverter(new CheckPlayerArea(stats, player, preventJump)),
                        new Selector(new List<Node>{
                            new CheckPlayerDirection(stats,player,Vector2.up, 1.6f, 200f),
                        }),
                        new Inverter(new CheckPlayerBehind(stats, player)),
                        new Inverter(new CheckRoof(stats)),
                        new EnemyJump(stats, body, jumpForce, forwardJumpForce),
                        new SetParentVariable("enableJump", false, 4),
                    }),


                    new Sequence(new List<Node>{
                        new CheckBool("enableJump", false),
                        new Wait(jumpIdleTime),
                        new SetParentVariable("enableJump", true, 4)
                    }),

                    new Sequence(new List<Node>{
                        new CheckWall(stats,Vector2.right, 1.5f,-.3f),
                        
                        new Selector(new List<Node>{
                            new Sequence(new List<Node>{
                                new CheckWall(stats,Vector2.right,1.5f, checkJumpHight),
                                new LookAtPlayer(stats, player)
                            }),

                            new Sequence(new List<Node>{
                                new CheckGrounded(stats,legPos),
                                new EnemyJump(stats, body, jumpForce, forwardJumpForce),
                                new SetParentVariable("enableJump", false, 4)
                            })
                        })
                    }),

                    new Sequence(new List<Node>{
                        
                        new CheckPlayerDistance(stats, player, 5, 20),
                        new CheckGrounded(stats, legPos),
                        new Inverter(new CheckPlayerArea(stats, player, preventJump)),
                        new AnimationBool(animator, "move", true),
                        new AnimationBool(animator, "run", true),
                        new Wait(.5f),
                        new LookAtPlayer(stats, player),
                        new RunForward(stats, runSpeed)
                    }),

                    new Sequence(new List<Node>{
                        new RunForward(stats, runSpeed),
                        new AnimationBool(animator, "move", true),
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
                new RandomPatroll(stats, body, animator, 1f, patrollIdleTime, legPos, "charge", "move")
            })
        });
        
        root.SetData("charge", false);
        root.SetData("enableJump", true);
        root.SetData("prusuit", false);
        root.SetData("crawlerAttack", true);

        triggersToFlip.Add(viewTrigger);
        triggersToFlip.Add(preventJump);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    protected override void DamageTaken(float damage, Vector2 atPostion)
    {
        if(root != null)
            root.SetData("prusuit", true);
    }
    protected override void Update()
    {
        base.Update();
        animator.SetInteger("velocityY", (int)body.velocity.y);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        viewTrigger.DrawTrigger(stats.GetPosition());
        preventJump.DrawTrigger(stats.GetPosition());
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.green;
        Handles.DrawDottedLine(stats.GetPosition()+Vector2.left+Vector2.up*checkJumpHight, stats.GetPosition()+Vector2.right+Vector2.up*checkJumpHight, 5);
    }
#endif
}
