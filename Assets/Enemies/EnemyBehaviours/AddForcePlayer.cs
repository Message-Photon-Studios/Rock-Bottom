using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AddForcePlayer : Node
{
    EnemyStats stats;
    Rigidbody2D player;
    Vector2 force;

    public AddForcePlayer(EnemyStats stats, PlayerStats player, Vector2 force)
    {
        this.stats = stats;
        this.player = player.GetComponent<Rigidbody2D>();
        this.force = force;
    }

    public override NodeState Evaluate()
    {
        int dir = (stats.GetPosition().x > player.transform.position.x)?-1:1;
        player.AddForce(force*(Vector2.right*dir+Vector2.up));
        state = NodeState.SUCCESS;
        return state;
    }
}
