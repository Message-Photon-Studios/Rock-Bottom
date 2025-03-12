using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class CrystalQuake : Enemy
{
    [SerializeField] float viewRange;
    [SerializeField] float eyePosY;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] Trigger quakeSmashTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] Trigger quakeTrigger;
    [SerializeField] GameObject quakeProjectile;
    [SerializeField] float quakeTimer;
    [SerializeField] Vector2 projectileSpawn1;
    [SerializeField] Vector2 projectileSpawn2;
    [SerializeField] Vector2 projectileSpawn3;
    [SerializeField] float projectileSpawnUpForce;
    [SerializeField] float projectileForceRandomness;
    [SerializeField] float patrollIdleTime;

    private float legPos = 1.8f;

    protected override Node SetupTree()
    {

        Node root = new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new CheckBool("attackDone", false),
                new NormalAttack("hornAttack", player, swordDamage, swordForce, 0.5f, attackTrigger, stats),

            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new CheckBool("stoneThrowAttack", true),
                new EnemyObjectSpawner(stats, quakeProjectile, projectileSpawn1, Vector2.up*projectileSpawnUpForce, false, projectileForceRandomness),
                new EnemyObjectSpawner(stats, quakeProjectile, projectileSpawn2, Vector2.up*projectileSpawnUpForce, false, projectileForceRandomness),
                new EnemyObjectSpawner(stats, quakeProjectile, projectileSpawn3, Vector2.up*projectileSpawnUpForce, false, projectileForceRandomness),
                new SetParentVariable("stoneThrowAttack", false, 2),
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", true),
                new CheckBool("attackDone", false),
                new NormalAttack("smashAttack", player, swordDamage, swordForce, .5f, quakeSmashTrigger, stats),
                new SetParentVariable("attackDone", true, 2)    
            }),

            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new CheckPlayerArea(stats, player, attackTrigger),
                new AnimationTrigger(animator, "attack")
            }),
            
            new Sequence(new List<Node>{
                new CheckBool("attack", false),
                new Wait(quakeTimer),
                new CheckPlayerArea(stats, player, quakeTrigger),
                new AnimationTrigger(animator, "quakeSlam")
            }),

            new Sequence(new List<Node>{
                new PlatformChase(stats, player, body, animator, 1f, viewRange, eyePosY, legPos ,"attack", "walk")
            }),

            new RandomPatroll(stats, body, animator, 1, patrollIdleTime, legPos, "attack", "walk")

            });

        root.SetData("attack", false);
        root.SetData("attackDone", false);
        root.SetData("hornAttack", false);
        root.SetData("stoneThrowAttack", false);
        root.SetData("smashAttack" , false);
        triggersToFlip.Add(attackTrigger);
        triggersToFlip.Add(quakeTrigger);
        triggersToFlip.Add(quakeSmashTrigger);
        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        attackTrigger.DrawTrigger(stats.GetPosition());
        quakeTrigger.DrawTrigger(stats.GetPosition());
        quakeSmashTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.blue;
        Handles.DrawLine(stats.GetPosition()+Vector2.left*viewRange+ Vector2.up*eyePosY, stats.GetPosition()+Vector2.right*viewRange+ Vector2.up*eyePosY);
        Handles.color = Color.green;
        Handles.DrawSolidDisc(stats.GetPosition()+projectileSpawn1, Vector3.forward, .1f);
        Handles.DrawSolidDisc(stats.GetPosition()+projectileSpawn2, Vector3.forward, .1f);
        Handles.DrawSolidDisc(stats.GetPosition()+projectileSpawn3, Vector3.forward, .1f);
    }
#endif
}
