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
    
    /// <summary>
    /// The normal attack damages and moves the player if the player is found within the trigger at the time of the specified bool is true. 
    /// If so it returns success and if any state failes it failes. 
    /// </summary>
    /// <param name="attackName"> The name of the bool that needs to be true to set the attack to active</param>
    /// <param name="player"></param>
    /// <param name="damage"> The damage that the attack will apply to the player.</param>
    /// <param name="force"> The force that will be applied to the player if the attack lands.</param>
    /// <param name="attackTrigger"> The area where the player needs to be to make the attack land.</param>
    /// <param name="stats"></param>
    /// <returns></returns>
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
        if(children[0].Evaluate() == NodeState.FAILURE) 
        {
            state = NodeState.FAILURE;
            return state;
        }

        var test = GetData(attackName);
        if(test != null && (bool) test)
        {
            player.DamagePlayer(damage);
            player.GetComponent<Rigidbody2D>().AddForce(((Vector2)player.transform.position + Vector2.up * 0.5f - attackTrigger.offset - stats.GetPosition()).normalized * force);
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
