using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Wait : Node
{
    float waitForSeconds;

    float timer;
    float lastUpdate;
    float tolerance;

    /// <summary>
    /// Only returns success after the timer has run out. After it returns success the timer resets. 
    /// Will return failure when the timer has not run out.
    /// </summary>
    /// <param name="waitForSeconds"></param>
    public Wait(float waitForSeconds)
    {
        this.waitForSeconds = waitForSeconds;
        this.tolerance = 0;
        this.timer = waitForSeconds;
        lastUpdate = -1;
        state = NodeState.SUCCESS;
    }
    /// <param name="tolerance">If this not is not evaluated within this many secounds after the timer finishes, reset the timer. 0 = unlimited</param>
    public Wait(float waitForSeconds, float tolerance)
    {
        this.waitForSeconds = waitForSeconds;
        this.tolerance = tolerance;
        this.timer = waitForSeconds;
        lastUpdate = -1;
        state = NodeState.SUCCESS;
    }

    public override NodeState Evaluate()
    {
        if(lastUpdate == -1) lastUpdate = Time.fixedTime;

        timer -= Time.fixedTime - lastUpdate;
        lastUpdate = Time.fixedTime;
        if(timer+tolerance <= 0 && tolerance != 0)
        {
            state = NodeState.FAILURE;
            timer = waitForSeconds;
            lastUpdate = -1;
        } else if(timer <= 0)
        {
            state = NodeState.SUCCESS;
            timer = waitForSeconds;
            lastUpdate = -1;
        } else
        {
            state = NodeState.FAILURE;
        }

        return state;
    }
}
