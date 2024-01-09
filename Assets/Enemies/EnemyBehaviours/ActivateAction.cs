using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System;
using UnityEditor.Events;
using UnityEngine.Events;

public class ActivateAction : Node
{
    UnityAction action;

    /// <summary>
    /// Activates the action and then returns
    /// </summary>
    /// <param name="action"></param>
    public ActivateAction(UnityAction action)
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
