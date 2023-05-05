using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class KeepHeight : Node
{
    EnemyStats stats;
    float height;
    float movementFactor;

    /// <summary>
    /// Tries to keep the enemy on a certain Y level
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="height"></param>
    /// <param name="movementFactor"></param>
    public KeepHeight(EnemyStats stats, float height, float movementFactor)
    {
        this.stats = stats;
        this.height = height;
        this.movementFactor = movementFactor;
    }

    public override NodeState Evaluate()
    {
        if(stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }

        if(Mathf.Abs(stats.GetPosition().y-height) < 0.2f)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        int dir = (stats.GetPosition().y - height < 0)?1:-1;
        stats.GetComponent<Rigidbody2D>().AddForce(Vector2.up*dir*stats.GetSpeed()*movementFactor);

        state = NodeState.RUNNING;
        return state;
    }

}
