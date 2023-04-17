using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyJump : Node
{
    EnemyStats stats;
    Rigidbody2D body;

    float jumpForce;
    float forwardForce;

    public EnemyJump(EnemyStats stats, Rigidbody2D body, float jumpForce, float forwardForce)
    {
        this.stats = stats;
        this.body = body;
        this.jumpForce = jumpForce;
        this.forwardForce = forwardForce;
    }
    public override NodeState Evaluate()
    {
        if(stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }

        body.AddForce(Vector2.up * jumpForce);
        body.AddForce(Vector2.right*stats.lookDir*forwardForce);
        state = NodeState.SUCCESS;
        return state;
    }
}
