using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class LookAtTarget : Node
{
    EnemyStats stats;
    string targetVar;
    public LookAtTarget(EnemyStats stats, string targetVar) : 
    base (new List<Node>{new CheckTargetBehind(stats, targetVar)})
    {
        this.stats = stats;
        this.targetVar = targetVar;
    }
    public override NodeState Evaluate()
    {
        if(children[0].Evaluate() == NodeState.SUCCESS)
        {
            stats.ChangeDirection();
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
