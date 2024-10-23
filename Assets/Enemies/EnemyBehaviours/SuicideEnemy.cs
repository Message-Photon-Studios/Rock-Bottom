using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

/// <summary>
/// The enemy kills itself
/// </summary>
public class SuicideEnemy : Node
{
    EnemyStats stats;

    public SuicideEnemy(EnemyStats stats)
    {
        this.stats = stats;
    }
    
    public override NodeState Evaluate()
    {
        stats.KillEnemy();
        state = NodeState.SUCCESS;
        return state;
    }
}
