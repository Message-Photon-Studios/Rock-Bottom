using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckGrounded : Node
{
    EnemyStats stats;
    float legPos;

    /// <summary>
    /// Returns success if the enemy is grounded
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="legPos"></param>
    public CheckGrounded (EnemyStats stats, float legPos)
    {
        this.stats = stats;
        this.legPos = legPos;
    }
    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition() + Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player", "Spell")) ||
                    Physics2D.Raycast(stats.GetPosition() - Vector2.right* legPos, Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player", "Spell"));
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
