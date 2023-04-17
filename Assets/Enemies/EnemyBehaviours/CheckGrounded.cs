using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckGrounded : Node
{
    EnemyStats stats;
    float legPos;
    public CheckGrounded (EnemyStats stats, float legPos)
    {
        this.stats = stats;
        this.legPos = legPos;
    }
    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition() + Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player")) ||
                    Physics2D.Raycast(stats.GetPosition() - Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player"));
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
