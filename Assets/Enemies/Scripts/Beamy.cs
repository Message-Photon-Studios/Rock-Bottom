using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class Beamy : Enemy
{
    [SerializeField] int damage = 0;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] ParticleSystem attackOrb;
    [SerializeField] ParticleSystem attackBeam;
    [SerializeField] ParticleSystem attackAim;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new Sequence(new List<Node>{

            new KeepHeight(stats, transform.position.y, 1f),

            new Selector(new List<Node>{
                
                new Sequence(new List<Node>{
                    new CheckBool("activateBeam", true),
                    new ParticlesPlay(attackOrb, true),
                    new ParticlesPlay(attackBeam, true),
                    new SetParentVariable("activateBeam", false, 3)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("sleeping", false),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new LookAtPlayer(stats, player),
                    new Selector(new List<Node>{ 
                        new CheckBool("attack", true),
                        new ParticlesPlay(attackAim, true)
                    }),
                    new SetParentVariable("attack", true, 3),
                    new AnimationBool(animator, "attack", true)
                    
                    }),

                new Sequence(new List<Node> {
                    new CheckBool("attack", true),
                    //new Wait(2),
                    new ParticlesPlay(attackOrb, false),
                    new ParticlesPlay(attackBeam, false),
                    new AnimationBool(animator, "attack", false),
                    new ParticlesPlay(attackAim, false),
                    new SetParentVariable("attack", false, 3)
                }),

                
                new AirPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .7f, "attack", "move")
            })
        });
        
        root.SetData("activateBeam", false);
        root.SetData("attack", false);
        root.SetData("sleeping", false);
        triggersToFlip.Add(attackTrigger);
        return root;
    }

    public void DamagePlayer()
    {
        player.DamagePlayer(damage);
    }

    public void ChooseBeamTarget()
    {
        GetComponentInChildren<BeamyBeam>().TargetSet();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
