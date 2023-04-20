using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckWall : Node
{
    EnemyStats stats;
    float distance;
    Vector2 direction;

    /// <summary>
    /// Retuns success if the enemy sees a wall in the given direction. The direction will in the X axis when the enemy turns.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    public CheckWall(EnemyStats stats, Vector2 direction, float distance)
    {
        this.stats = stats;
        this.direction = direction;
        this.distance = distance;
    }

    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition(), new Vector2(direction.x*stats.lookDir, direction.y), distance, ~LayerMask.GetMask("Enemy", "Player", "Spell"));
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
