using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerDirection : Node
{
    EnemyStats stats;
    Transform player;
    float minDist;
    float maxDist;

    Vector2 direction;

    /// <summary>
    /// Returns success if the player is within the given bounds in the direction that is specified. The direction flips in the X axis when the enemy turns
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <param name="minDist"></param>
    /// <param name="maxDist"></param>
    public CheckPlayerDirection(EnemyStats stats, PlayerStats player,Vector2 direction, float minDist, float maxDist)
    {
        this.stats = stats;
        this.player = player.transform;
        this.direction = direction;
        this.minDist = minDist;
        this.maxDist = maxDist;
    }

    public override NodeState Evaluate()
    {
        float distSqr = (((Vector2)player.position-stats.GetPosition())*new Vector2(stats.lookDir,1)*direction.normalized).magnitude;

        state = (distSqr < minDist || distSqr > maxDist)?NodeState.FAILURE:NodeState.SUCCESS;
        Debug.Log("Checkdir " + direction +" : "+ state);

        return state;

    }
}
