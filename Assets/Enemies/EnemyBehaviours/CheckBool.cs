using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckBool : Node
{
    string boolName;
    bool equals;
    public CheckBool (string boolName, bool equals)
    {
        this.boolName = boolName;
        this.equals = equals;
    }
    public override NodeState Evaluate()
    {
        var test = GetData(boolName);
        if(test == null) return NodeState.FAILURE;
        return ((bool)test == equals)?NodeState.SUCCESS:NodeState.FAILURE;
    }
}
