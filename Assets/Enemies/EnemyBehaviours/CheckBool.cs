using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckBool : Node
{
    string boolName;
    public CheckBool (string boolName)
    {
        this.boolName = boolName;
    }
    public override NodeState Evaluate()
    {
        var test = GetData(boolName);
        if(test == null) return NodeState.FAILURE;
        return (bool)test?NodeState.SUCCESS:NodeState.FAILURE;
    }
}
