using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class DamagePlayer : Node
{
    EnemyStats stats;
    PlayerStats player;
    int damage;
    
    /// <summary>
    /// The DamagePlayer damages the player with the specified damage and then returns success
    /// </summary>
    /// <param name="player"></param>
    /// <param name="damage"></param>
    public DamagePlayer (EnemyStats stats, PlayerStats player, int damage)
    {
        this.player = player;
        this.damage = damage;
        this.stats = stats;
    }
    public override NodeState Evaluate()
    {
        if (!stats.IsAsleep() && !stats.IsDead())
        {
            player.DamagePlayer(stats.GetScaledDamage(damage), stats);
            state = NodeState.SUCCESS;
        } else state = NodeState.FAILURE;
        return state;
    }
}
