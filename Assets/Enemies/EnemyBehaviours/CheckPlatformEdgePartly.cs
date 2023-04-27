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
        bool test = !Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player","Spell", "Ignore Raycast", "Item")) ||
                    Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos, Vector2.right * (legPos), .2f, ~LayerMask.GetMask("Player", "Enemy","Spell", "Ignore Raycast", "Item"));
        state = (test)?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
