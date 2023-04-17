using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Wait : Node
{
    float waitForSeconds;

    float timer;
    float lastUpdate;
    public Wait(float waitForSeconds)
    {
        this.waitForSeconds = waitForSeconds;
        this.timer = waitForSeconds;
        lastUpdate = -1;
        state = NodeState.SUCCESS;
    }

    public override NodeState Evaluate()
    {
        if(lastUpdate == -1) lastUpdate = Time.fixedTime;

        timer -= Time.fixedTime - lastUpdate;
        lastUpdate = Time.fixedTime;
        if(timer <= 0)
        {
            state = NodeState.SUCCESS;
            timer = waitForSeconds;
        } else
        {
            state = NodeState.FAILURE;
        }

        return state;
    }
}
