using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckTargetDistance : Node
{
    string targetVar;
    EnemyStats stats;
    float distance;

    /// <summary>
    /// Returns success if the target is within the specified distance
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="targetVar">The name of the variable with the target transform</param>
    public CheckTargetDistance(EnemyStats stats, string targetVar, float distance)
    {
        this.stats = stats;
        this.targetVar = targetVar;
        this.distance = distance;

    }

    public override NodeState Evaluate()
    {
        GameObject target = GetData(targetVar) as GameObject;
        state = (Vector2.Distance(target.transform.position, stats.GetPosition()) < distance)? NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
