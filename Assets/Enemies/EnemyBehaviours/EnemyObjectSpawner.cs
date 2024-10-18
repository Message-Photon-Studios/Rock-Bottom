using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyObjectSpawner : Node
{
    EnemyStats stats;
    GameObject spawnTemp;
    Vector2 offset;
    Vector2 force;
    float forceRandomVariation;
    bool setColor;
    /// <summary>
    /// Spawns an istance of the spawnTemp object at the enemys position + the offset.
    /// The offset flips with the enemy look direction. 
    /// The object will have a force applied to it equal to the force and also flipped with the enemy look dir.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="spawnTemp"></param>
    /// <param name="offset"></param>
    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset, Vector2 force, bool setColor, float forceRandomVariation)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.force = force;
        this.forceRandomVariation = forceRandomVariation;
        this.setColor = setColor;
    }

    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset, Vector2 force, bool setColor)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.force = force;
        this.forceRandomVariation = 0f;
        this.setColor = setColor;
    }

    public override NodeState Evaluate()
    {
        Vector2 useOffset = offset * (Vector2.left*stats.lookDir + Vector2.up);
        if(stats && spawnTemp && offset!=null && force!=null)
        {
            GameObject spwn = GameObject.Instantiate(spawnTemp, stats.GetPosition()+useOffset, stats.gameObject.transform.rotation) as GameObject;
            spwn.GetComponent<Rigidbody2D>()?.AddForce(force*(Vector2.right*stats.lookDir+Vector2.up)*(Random.Range(0f,forceRandomVariation)+1f));

            EnemyStats spwnStats = spwn.GetComponent<EnemyStats>();
            if(spwnStats) spwnStats.spawnPower = stats.GetDamageFactor();
            if(setColor) spwn.GetComponent<EnemyStats>()?.SetColor(stats.GetColor());
            else if(setColor) spwn.GetComponent<EnemyStats>()?.SetColor(null);
        }   
        state = NodeState.SUCCESS;
        return state;
    }
}
