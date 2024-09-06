using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

/// <summary>
/// Returns success if Enemy is sleeping
/// </summary>
public class IsSleeping : Node
{
    private EnemyStats stats;
    public IsSleeping(EnemyStats stats)
    {
        this.stats = stats;
    }

    public override NodeState Evaluate()
    {
        if(stats.IsAsleep()) state = NodeState.SUCCESS;
        else state = NodeState.FAILURE;

        return state;
    }
}
