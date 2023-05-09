using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyObjectSpawnerAim : Node
{
    EnemyStats stats;
    GameObject spawnTemp;
    Vector2 offset;
    PlayerStats player;
    float force;

    /// <summary>
    /// Spawns an istance of the spawnTemp object at the enemys position + the offset.
    /// The offset flips with the enemy look direction. 
    /// The object will have a force applied to it equal to the force and also flipped with the enemy look dir.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="spawnTemp"></param>
    /// <param name="offset"></param>
    public EnemyObjectSpawnerAim(EnemyStats stats, GameObject spawnTemp, Vector2 offset, PlayerStats player, float force)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.player = player;
        this.force = force;
    }

    public override NodeState Evaluate()
    {
        float dX = player.transform.position.x - (stats.GetPosition().x + offset.x);
        float dY = player.transform.position.y - (stats.GetPosition().y + offset.y);
        float deg = Mathf.Atan2(dY, dX);


        Vector2 useOffset = offset * (Vector2.left * stats.lookDir + Vector2.up);

        GameObject spwn = GameObject.Instantiate(spawnTemp, stats.GetPosition()+useOffset, stats.gameObject.transform.rotation) as GameObject;
        
        spwn.transform.rotation = Quaternion.EulerAngles(0,0,deg);

        spwn.GetComponent<Rigidbody2D>()?.AddForce(spwn.transform.localToWorldMatrix.MultiplyPoint((force * Vector2.right)));


        spwn.GetComponent<EnemyStats>()?.SetColor(stats.GetColor());
        state = NodeState.SUCCESS;
        return state;
    }
}
