using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerDistance : Node
{
    EnemyStats stats;
    Transform player;
    float minDistanceSqr;
    float maxDistanceSqr;

    public CheckPlayerDistance(EnemyStats stats, PlayerStats player, float minDistance, float maxDistance)
    {
        this.stats = stats;
        this.player = player.transform;
        this.minDistanceSqr = minDistance*minDistance;
        this.maxDistanceSqr = maxDistance*maxDistance;
    }

    public override NodeState Evaluate()
    {
        float dstSqr = (stats.GetPosition() - (Vector2)player.position).sqrMagnitude;

        state = (dstSqr < minDistanceSqr || dstSqr > maxDistanceSqr)?NodeState.FAILURE:NodeState.SUCCESS;
        return state;
    }
}
