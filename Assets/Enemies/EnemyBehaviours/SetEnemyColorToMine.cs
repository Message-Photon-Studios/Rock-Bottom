using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class SetEnemyColorToMine : Node
{
    string targetVar;
    EnemyStats stats;


    /// <summary>
    /// Sets the targeted var enemys color to my color. Returns failure if the target doesn't have an enemy stats component
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="targetVar">The name of the variable with the target game object</param>
    public SetEnemyColorToMine(EnemyStats stats, string targetVar)
    {
        this.stats = stats;
        this.targetVar = targetVar;

    }

    public override NodeState Evaluate()
    {
        GameObject target = GetData(targetVar) as GameObject;
        
        EnemyStats targetStats = target.GetComponent<EnemyStats>();
        if(!targetStats)
        {
            state = NodeState.FAILURE;
            return state;
        } 
        targetStats.SetColor(stats.GetColor(), stats.GetColorAmmount());


        state = NodeState.SUCCESS;
        return state;
    }
}
