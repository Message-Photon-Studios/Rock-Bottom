using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlatformEdge : Node
{
    EnemyStats stats;
    float legPos;

    public CheckPlatformEdge(EnemyStats stats, float legPos)
    {
        this.stats = stats;
        this.legPos = legPos;
    }
    public override NodeState Evaluate()
    {
        bool test = !Physics2D.Raycast(stats.GetPosition() + Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player")) ||
                    !Physics2D.Raycast(stats.GetPosition() - Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player")) ||
                    Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos, Vector2.right, 1f, ~LayerMask.GetMask("Player")) ||
                    Physics2D.Raycast(stats.GetPosition() - Vector2.right * legPos, Vector2.left, 1f, ~LayerMask.GetMask("Player"));
        return (test)?NodeState.SUCCESS:NodeState.FAILURE;
    }
}
