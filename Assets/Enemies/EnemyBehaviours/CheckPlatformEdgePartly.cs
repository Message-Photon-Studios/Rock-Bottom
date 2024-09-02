using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

/// <summary>
/// Only checks the edge of the platform on one side
/// </summary>
public class CheckPlatformEdgePartly : Node
{
    EnemyStats stats;
    float legPos;

    /// <summary>
    /// Only checks the edge of the platform on one side, decided by if the legPos is larger or smaller than zero.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="legPos"></param>
    public CheckPlatformEdgePartly(EnemyStats stats, float legPos)
    {
        this.stats = stats;
        this.legPos = legPos;
    }

    public override NodeState Evaluate()
    {
        bool test = !Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos*stats.lookDir, Vector2.down, 2f, GameManager.instance.maskLibrary.onlyGround) ||
                    Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos*stats.lookDir, Vector2.right * (legPos*stats.lookDir), .2f, GameManager.instance.maskLibrary.onlyGround);
        state = (test)?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
