using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

public class ActivateAction<T> : Node
{
    UnityEvent<T> action;
    string varName;

    /// <summary>
    /// Activates the action and then returns
    /// </summary>
    /// <param name="action"></param>
    public ActivateAction(UnityEvent<T> action, string varName)
    {
        this.action = action;
        this.varName = varName;
    }

    public override NodeState Evaluate()
    {
        
        if(action != null) action?.Invoke((T)GetData(varName));
        state = NodeState.SUCCESS;
        return state;
    }
}

public class ActivateAction : Node
{
    UnityEvent action;
    
    public ActivateAction(UnityEvent action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        if(action != null) action.Invoke();
        state = NodeState.SUCCESS;
        return state;
    }
}
