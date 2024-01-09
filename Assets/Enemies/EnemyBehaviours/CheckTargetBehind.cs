using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckTargetBehind : Node
{
    string targetVar;
    EnemyStats stats;

    /// <summary>
    /// Returns success if the player is behind the enemy
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="targetVar">The name of the variable with the target transform</param>
    public CheckTargetBehind(EnemyStats stats, string targetVar)
    {
        this.stats = stats;
        this.targetVar = targetVar;
    }

    public override NodeState Evaluate()
    {
        GameObject target = GetData(targetVar) as GameObject;
        state = ((target.transform.position.x - stats.GetPosition().x)*stats.lookDir<0)? NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
