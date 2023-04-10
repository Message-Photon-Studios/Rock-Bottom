using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class SetParentVariable : Node
{
    object value;
    string key;
    int backSteps;

    /// <summary>
    /// The SetParentVariable will try and set the data of a specified key on a parent object. If it succeds it will return success.
    /// It will return faliure if it cant find the parent that it wants to set the data on. 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="backSteps">
    /// This determines how far up the tree it will search to find the parent that it sets the data on. 
    /// Setting this to zero will set the data on this node.
    /// </param>
    public SetParentVariable(string key, object value, int backSteps)
    {
        this.key = key;
        this.value = value;
        this.backSteps = backSteps;
    }
    public override NodeState Evaluate()
    {
        Node node = this;
        for (int i = 0; i < backSteps; i++)
        {
            node = node.parent;
            if(node == null) 
            {
                state = NodeState.FAILURE;
            }
        }

        node.SetData(key, value);
        state = NodeState.SUCCESS;
        return state;
    }
}
