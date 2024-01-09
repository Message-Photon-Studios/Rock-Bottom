using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System;

public class ActivateAction : Node
{
    Action action;

    /// <summary>
    /// Activates the action and then returns
    /// </summary>
    /// <param name="action"></param>
    public ActivateAction(Action action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Activate Action");
        action?.Invoke();
        state = NodeState.SUCCESS;
        return state;
    }
}
