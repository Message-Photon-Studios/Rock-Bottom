using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlatformEdge : Node
{
    EnemyStats stats;
    float legPos;

    /// <summary>
    /// The CheckPlatformEdge will return success if a wall, enemy or edge is found near the enemy
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="legPos">The distace from the enemys center to a leg check</param>
    public CheckPlatformEdge(EnemyStats stats, float legPos)
    {
        this.stats = stats;
        this.legPos = legPos;
    }
    public override NodeState Evaluate()
    {
        bool test = !Physics2D.Raycast(stats.GetPosition() + Vector2.right* legPos, Vector2.down, 1.5f, GameManager.instance.maskLibrary.onlyGround) ||
                    !Physics2D.Raycast(stats.GetPosition() - (Vector2.right* legPos), Vector2.down, 1.5f, GameManager.instance.maskLibrary.onlyGround) ||
                    Physics2D.Raycast(stats.GetPosition() + Vector2.right * legPos, Vector2.right, .2f, GameManager.instance.maskLibrary.onlyGround) ||
                    Physics2D.Raycast(stats.GetPosition() - (Vector2.right * legPos), Vector2.left, .2f, GameManager.instance.maskLibrary.onlyGround);
        state = (test)?NodeState.SUCCESS:NodeState.FAILURE;;
        return state;
    }
}
