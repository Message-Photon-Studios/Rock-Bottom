using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class DamagePlayer : Node
{
    PlayerStats player;
    float damage;
    
    public DamagePlayer (PlayerStats player, float damage)
    {
        this.player = player;
        this.damage = damage;
    }
    public override NodeState Evaluate()
    {
        player.DamagePlayer(damage);
        return NodeState.SUCCESS;
    }
}
