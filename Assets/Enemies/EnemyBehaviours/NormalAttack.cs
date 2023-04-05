using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class NormalAttack : Node
{
    PlayerStats player;
    float damage;
    float force;
    string attackName;
    Trigger attackTrigger;
    EnemyStats stats;
    
    public NormalAttack (string attackName, PlayerStats player, float damage, float force, Trigger attackTrigger, EnemyStats stats) : 
        base(new List<Node>{new CheckPlayerArea(stats, player, attackTrigger)})
    {
        this.attackName = attackName;
        this.force = force;
        this.player = player;
        this.damage = damage;
        this.attackTrigger = attackTrigger;
        this.stats = stats;
    }
    public override NodeState Evaluate()
    {
        if(children[0].Evaluate() == NodeState.FAILURE) return NodeState.FAILURE;

        var test = GetData(attackName);
        if(test != null && (bool) test)
        {
            player.DamagePlayer(damage);
            player.GetComponent<Rigidbody2D>().AddForce(((Vector2)player.transform.position - attackTrigger.offset - stats.GetPosition()) * force);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
