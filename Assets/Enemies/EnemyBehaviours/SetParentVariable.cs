using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class SetParentVariable : Node
{
    object value;
    string key;
    int backSteps;
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
            if(node == null) return NodeState.FAILURE;
        }

        node.SetData(key, value);
        return NodeState.SUCCESS;
    }
}
