using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
public class CheckBool : Node
{
    string boolName;
    bool equals;
    /// <summary>
    /// The CheckBool will return success if it finds a bool that matches the equallity. It will retunr false if no bool was found or if
    /// the bool doesn't match the equallity.
    /// </summary>
    /// <param name="boolName"></param>
    /// <param name="equals"></param>
    public CheckBool (string boolName, bool equals)
    {
        this.boolName = boolName;
        this.equals = equals;
    }
    public override NodeState Evaluate()
    {
        var test = GetData(boolName);
        if(test == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        state = ((bool)test == equals)?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
