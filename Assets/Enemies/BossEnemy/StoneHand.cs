using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using UnityEngine.Events;

public class StoneHand : Enemy
{
    [SerializeField] GameObject stoneProjectile;
    [SerializeField] float stoneUpForce;
    [SerializeField] float stoneRadomForce;
    [SerializeField] float stonesHpSpawn;
    [SerializeField] Vector2 spawnOffset;

        
    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new CheckBool("sleeping", false),
            new CheckBool("stoneThrowAttack", true),
            new EnemyObjectSpawner(stats, stoneProjectile, spawnOffset, Vector2.up*stoneUpForce, false, stoneRadomForce),
            new SetParentVariable("stoneThrowAttack", false, 1),
        });

        root.SetData("stoneThrowAttack", false);
        root.SetData("sleeping", false);
        return root;
    }
}
