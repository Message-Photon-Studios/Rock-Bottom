using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree; 

public class CheckRoof : Node
{
    EnemyStats stats;
    float distance;

    public CheckRoof(EnemyStats stats)
    {
        this.stats = stats;
        this.distance = 2f;
    }
    public CheckRoof(EnemyStats stats, float distance)
    {
        this.stats = stats;
        this.distance = distance;
    }

    /// <summary>
    /// Returns success if the enemy has a roof above it
    /// </summary>
    /// <returns></returns>
    public override NodeState Evaluate()
    {
        bool test = Physics2D.Raycast(stats.GetPosition(), Vector2.up, distance, GameManager.instance.maskLibrary.onlyGround);
        state = test?NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
