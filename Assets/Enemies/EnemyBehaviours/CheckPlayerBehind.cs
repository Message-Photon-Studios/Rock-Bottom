using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerBehind : Node
{
    Transform player;
    EnemyStats stats;

    /// <summary>
    /// Returns success if the player is behind the enemy
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="player"></param>
    public CheckPlayerBehind(EnemyStats stats, PlayerStats player)
    {
        this.stats = stats;
        this.player = player?.transform;
    }

    public override NodeState Evaluate()
    {
        state = ((player.position.x - stats.GetPosition().x)*stats.lookDir<0)? NodeState.SUCCESS:NodeState.FAILURE;
        return state;
    }
}
