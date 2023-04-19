using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree; 

public class CheckRoof : Node
{
    EnemyStats stats;

    public CheckRoof(EnemyStats stats)
    {
        this.stats = stats;
    }

    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition(), Vector2.up, 2f, ~LayerMask.GetMask("Enemy", "Player", "Spell"));
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
