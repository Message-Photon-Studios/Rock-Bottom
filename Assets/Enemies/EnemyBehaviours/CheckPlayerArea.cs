using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerArea : Node
{
    EnemyStats stats;
    PlayerStats player;
    Trigger trigger;

    /// <summary>
    /// The CheckPlayerArea will return if the player is found within the bounds of the trigger and if the FOV from the trigger to the player isn't obstructed.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="player"></param>
    /// <param name="trigger"></param>
    public CheckPlayerArea(EnemyStats stats, PlayerStats player, Trigger trigger)
    {
        this.stats = stats;
        this.player = player;
        this.trigger = trigger;
    }

    public override NodeState Evaluate()
    {
        var hit = Physics2D.Raycast(stats.GetPosition() + trigger.offset, (Vector2)player.transform.position -  trigger.offset - stats.GetPosition(), trigger.radius, ~LayerMask.GetMask("Enemy","Spell", "Ignore Raycast"));
        state = (!stats.IsAsleep() && hit.collider != null && hit.collider.CompareTag("Player"))? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
