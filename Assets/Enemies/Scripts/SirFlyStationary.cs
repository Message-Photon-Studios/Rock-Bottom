using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

/// <summary>
/// This is the ai for the Crystoffer enemy
/// </summary>
public class SirFlyStationary : Enemy
{
    [SerializeField] int spikesDamage;
    [SerializeField] Trigger damageTrigger;
    [SerializeField] Trigger rangeTrigger;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] GameObject attackSpawn;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] float spawnForce;
    [SerializeField] ParticleSystem aimTarget;

    protected override Node SetupTree()
    {

        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("inRange", true),
                new CheckPlayerArea(stats, player, damageTrigger),
                new DamagePlayer(stats, player, spikesDamage)
            }),
            new Sequence(new List<Node>{
                new CheckBool("attackDone", true),
                new EnemyObjectSpawnerAim(stats, attackSpawn, spawnOffset, player, spawnForce),
                new SetParentVariable("attackDone", false, 2),
                new ParticlesPlay (aimTarget, false)
            }),
            new Sequence(new List<Node>{
                new CheckBool("inRange", true),
                new CheckPlayerArea(stats, player, attackTrigger),
                new AnimationTrigger(animator,"attack"),
                new SetParentVariable("inRange", false, 2),
                new SetParentVariable("attackReady", false, 2),
                new ParticlesPlay (aimTarget, false)
            }),
            new Sequence(new List<Node>{
                new Inverter( new CheckPlayerArea(stats, player, rangeTrigger)),
                new ParticlesPlay (aimTarget, false),
                new CheckBool("inRange", true),
                new SetParentVariable("inRange", false, 2),
                new AnimationBool(animator, "inRange", false)
            }),
            new Sequence(new List<Node>{
                new CheckBool("inRange", false),
                new CheckPlayerArea(stats, player, rangeTrigger),
                new AnimationBool(animator, "inRange",true),
                new ParticlesPlay (aimTarget, true)
                })
            }); 
        
        root.SetData("inRange", false);
        root.SetData("attackReady", false);
        root.SetData("attackDone", false);
        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(damageTrigger);
        return root;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        rangeTrigger.DrawTrigger(stats.GetPosition());
        damageTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
    }
#endif
}
