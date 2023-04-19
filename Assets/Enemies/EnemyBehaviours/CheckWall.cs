using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckWall : Node
{
    EnemyStats stats;
    float distance;
    Vector2 direction;

    public CheckWall(EnemyStats stats, Vector2 direction, float distance)
    {
        this.stats = stats;
        this.direction = direction;
        this.distance = distance;
    }

    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition(), new Vector2(direction.x*stats.lookDir, direction.y), distance, ~LayerMask.GetMask("Enemy", "Player", "Spell"));
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
