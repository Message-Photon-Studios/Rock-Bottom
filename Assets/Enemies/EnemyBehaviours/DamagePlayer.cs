using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class DamagePlayer : Node
{
    PlayerStats player;
    float damage;
    
    /// <summary>
    /// The DamagePlayer damages the player with the specified damage and then returns success
    /// </summary>
    /// <param name="player"></param>
    /// <param name="damage"></param>
    public DamagePlayer (PlayerStats player, float damage)
    {
        this.player = player;
        this.damage = damage;
    }
    public override NodeState Evaluate()
    {
        player.DamagePlayer(damage);
        state = NodeState.SUCCESS;
        return state;
    }
}
