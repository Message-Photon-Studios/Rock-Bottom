using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class RunForward : Node
{
    EnemyStats stats;
    Rigidbody2D body;
    float speedFactor;

    /// <summary>
    /// This node makes the enemy run forward as long as it isn't asleep.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="speedFactor">This is multiplied with the enemy movement speed.</param>
    public RunForward(EnemyStats stats, float speedFactor)
    {
        this.stats = stats;
        this.body = stats.GetComponent<Rigidbody2D>();
        this.speedFactor = speedFactor;
    }

    public override NodeState Evaluate()
    {
        if(stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }

        body.AddForce(new Vector2(stats.lookDir * stats.GetSpeed() * speedFactor, 0f)*Time.fixedDeltaTime);
        state = NodeState.SUCCESS;
        return state; 
    }
}
