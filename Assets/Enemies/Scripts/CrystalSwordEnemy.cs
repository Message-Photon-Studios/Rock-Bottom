using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSwordEnemy : Enemy
{
    [SerializeField] Trigger attackTrigger;
    [SerializeField] float swordDamage;
    [SerializeField] float swordForce;

    protected override void Update() {
        base.Update();
        if(CheckTrigger(attackTrigger))
        {
            animator.SetTrigger("attack");
        }
    }

    private void OnDrawGizmosSelected() {
        attackTrigger.DrawTrigger(GetPosition());
    }

    public void SwordHit() 
    {
        if(CheckTrigger(attackTrigger))
        {
            player.DamagePlayer(swordDamage);
            player.GetComponent<Rigidbody2D>().AddForce(((Vector2)player.transform.position - attackTrigger.offset - GetPosition()) * swordForce);
        }
    }
}
