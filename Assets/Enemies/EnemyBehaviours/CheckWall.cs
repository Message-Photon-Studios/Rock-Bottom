using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckWall : Node
{
    EnemyStats stats;
    float distance;
    Vector2 direction;
    float yPos;

    /// <summary>
    /// Retuns success if the enemy sees a wall in the given direction. The direction will in the X axis when the enemy turns.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    public CheckWall(EnemyStats stats, Vector2 direction, float distance, float yPos)
    {
        this.stats = stats;
        this.direction = direction;
        this.distance = distance;
        this.yPos = yPos;
    }

    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition()+ Vector2.up*yPos, new Vector2(direction.x*stats.lookDir, direction.y), distance, GameManager.instance.maskLibrary.onlyGround);
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
