using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class RunForward : Node
{
    EnemyStats stats;
    Rigidbody2D body;
    float speedFactor;

    public RunForward(EnemyStats stats, float speedFactor)
    {
        this.stats = stats;
        this.body = stats.GetComponent<Rigidbody2D>();
        this.speedFactor = speedFactor;
    }

    public override NodeState Evaluate()
    {
        body.AddForce(new Vector2(stats.lookDir * stats.GetSpeed() * speedFactor, 0f)*Time.fixedDeltaTime);
        state = NodeState.SUCCESS;
        return state; 
    }
}
