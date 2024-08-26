using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckPlayerAreaCone : Node
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
    public CheckPlayerAreaCone(EnemyStats stats, PlayerStats player, Trigger trigger)
    {
        this.stats = stats;
        this.player = player;
        this.trigger = trigger;
    }

    public override NodeState Evaluate()
    {
        var hit = Physics2D.Raycast(stats.GetPosition() + trigger.offset, (Vector2)player.transform.position -  trigger.offset - stats.GetPosition(), trigger.radius, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlayer);
        state = (!stats.IsAsleep() && hit.collider != null && hit.collider.CompareTag("Player") && inside())? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }

    private bool inside()
    {
        float dX = player.transform.position.x - (stats.GetPosition().x + trigger.offset.x);
        float dY = player.transform.position.y - (stats.GetPosition().y + trigger.offset.y);
        float deg = Mathf.Atan2(dY, dX) * Mathf.Rad2Deg;
        if ((trigger.direction - deg) % 360 <= trigger.width / 2) return true;
        return false;
    }
}
