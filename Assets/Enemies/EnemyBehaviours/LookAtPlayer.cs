using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class LookAtPlayer : Node
{
    EnemyStats stats;
    Transform player;
    public LookAtPlayer(EnemyStats stats, PlayerStats player) : 
    base (new List<Node>{new CheckPlayerBehind(stats, player)})
    {
        this.stats = stats;
        this.player = player?.gameObject.transform;
    }
    public override NodeState Evaluate()
    {
        if(stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }
        if(children[0].Evaluate() == NodeState.SUCCESS)
        {
            stats.ChangeDirection();
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
