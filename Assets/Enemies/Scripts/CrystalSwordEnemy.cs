using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CrystalSwordEnemy : Enemy
{   
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float swordDamage;
    [SerializeField] float swordForce;

    protected override Node SetupTree()
    {
        
        Node root = new Selector(new List<Node>{
            new NormalAttack("swordAttack", player, swordDamage, swordForce, attackTrigger, stats),
            new Sequence(new List<Node>{
                new CheckPlayerArea(stats, player, attackTrigger),
                new PlayAttack(animator, "attack")
                })
            });
        return root;
    }

    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(stats.GetPosition());
    }

    /*public void SwordHit() 
    {
        if(CheckTrigger(attackTrigger))
        {
            player.DamagePlayer(swordDamage);
            player.GetComponent<Rigidbody2D>().AddForce(((Vector2)player.transform.position - attackTrigger.offset - stats.GetPosition()) * swordForce);
        }
    }*/
}
