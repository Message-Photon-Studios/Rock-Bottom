using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerArea : Node
{
    EnemyStats stats;
    PlayerStats player;
    Trigger trigger;

    public CheckPlayerArea(EnemyStats stats, PlayerStats player, Trigger trigger)
    {
        this.stats = stats;
        this.player = player;
        this.trigger = trigger;
    }

    public override NodeState Evaluate()
    {
        var hit = Physics2D.Raycast(stats.GetPosition() + trigger.offset, (Vector2)player.transform.position -  trigger.offset - stats.GetPosition(), trigger.radius);
        return (!stats.IsAsleep() && hit.collider != null && hit.collider.CompareTag("Player"))? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
