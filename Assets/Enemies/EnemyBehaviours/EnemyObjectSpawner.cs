using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyObjectSpawner : Node
{
    EnemyStats stats;
    GameObject spawnTemp;
    Vector2 offset;

    /// <summary>
    /// Spawns an istance of the spawnTemp object at the enemys position + the offset.
    /// The offset flips with the enemy look direction.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="spawnTemp"></param>
    /// <param name="offset"></param>
    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
    }

    public override NodeState Evaluate()
    {
        Vector2 useOffset = offset * (Vector2.right*stats.lookDir + Vector2.up);
        GameObject.Instantiate(spawnTemp, stats.GetPosition()+useOffset, stats.gameObject.transform.rotation);
        state = NodeState.SUCCESS;
        return state;
    }
}
