using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class BossHand : Enemy
{
    [SerializeField] float viewRange;
    [SerializeField] Trigger attackTrigger;
    [SerializeField] int swordDamage;
    [SerializeField] float swordForce;
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;
    [SerializeField] float spellCastingCooldown;
    [SerializeField] GameObject bossSpellTemplate;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] Vector2 spawnForce;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{

            new Sequence(new List<Node>{
                new CheckBool("attackDone", false),
                new NormalAttack("swordAttack", player, swordDamage, swordForce, 0.5f, attackTrigger, stats),
                new SetParentVariable("attackDone", true, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("castSpell", true),
                new EnemyObjectSpawner(stats, bossSpellTemplate, spawnOffset, spawnForce, true),
                new SetParentVariable("castSpell", false, 2)
            }),

            new Sequence(new List<Node>{
                new CheckBool("idle", true),
                new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")
            }),

            new Sequence(new List<Node>{
            new CheckBool("spellMode", true),
            new Selector(new List<Node>
            {
                new Sequence(new List<Node>{
                    new Wait(spellCastingCooldown),
                    new AnimationTrigger(animator, "spell")
                }),
                new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")
            })
            }),

            new Selector(new List<Node>{
                new Sequence(new List<Node>{
                    new CheckBool("attack", false),
                    new CheckPlayerArea(stats, player, attackTrigger),
                    new AnimationTrigger(animator, "attack")
                    }),

                new Sequence(new List<Node>{
                    new PlatformChase(stats, player, body, animator, 1f, viewRange, 0f, .5f ,"attack", "walk")
                }),

                new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .5f, "attack", "walk")
            })
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.blue;
        Handles.DrawLine(stats.GetPosition()+Vector2.left*viewRange, stats.GetPosition()+Vector2.right*viewRange);
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
