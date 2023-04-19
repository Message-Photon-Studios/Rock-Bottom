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
        float distSqr = (((Vector2)player.position-stats.GetPosition())*direction.normalized).sqrMagnitude;

        state = (distSqr < minDist || distSqr > maxDist)?NodeState.FAILURE:NodeState.SUCCESS;

        return state;

    }
}
