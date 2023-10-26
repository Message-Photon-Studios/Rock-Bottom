using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class SetObejctActive: Node
{
    GameObject obj;
    bool active = false;

    /// <summary>
    /// Sets an object to active or inactive
    /// </summary>
    public SetObejctActive(GameObject obj, bool active)
    {
        this.obj = obj;
        this.active = active;
    }

    public override NodeState Evaluate()
    {
        obj.SetActive(active);
        return NodeState.SUCCESS;
    }
}
