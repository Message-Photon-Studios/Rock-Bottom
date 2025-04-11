using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The red color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/RedColorEffect")]
public class RedColorEffect : ColorEffect
{
    [SerializeField] float force;
    [SerializeField] GameObject orb;
    [SerializeField] float healingPercent;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats player = playerObj.GetComponent<PlayerStats>();

        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;

        if (!enemy.IsKnockbackImune())
        {
            Vector3 pullPoint = (forcePerspectivePlayer)?playerObj.transform.position : new Vector3(impactPoint.x, impactPoint.y, enemy.transform.position.z);
            enemy?.GetComponent<Rigidbody2D>()?.AddForce((pullPoint - enemy.transform.position).normalized * force);
        }
        float enemyHP = enemy.GetHealth() - Mathf.RoundToInt(damage * power);
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power)+extraDamage);
        //player.HealPlayer(Mathf.RoundToInt(healing*power));
        int healing = 0;
        if (enemyHP >= 0)
        {
            healing = Mathf.RoundToInt(damage * power * healingPercent);
        } else
        {
            healing = Mathf.RoundToInt(((damage * power) + enemyHP) * healingPercent);
        }
        while(healing > 0)
        {
            if (healing >= 5)
            {
                GameObject orb1 = GameObject.Instantiate(orb, enemyObj.transform.position, Quaternion.identity);
                orb1.GetComponent<HealingOrb>().SetTarget(playerObj, 5);
                healing -= 5;
            } else if (healing >= 2)
            {
                GameObject orb1 = GameObject.Instantiate(orb, enemyObj.transform.position, Quaternion.identity);
                orb1.GetComponent<HealingOrb>().SetTarget(playerObj, 2);
                healing -= 2;
            } else
            {
                GameObject orb1 = GameObject.Instantiate(orb, enemyObj.transform.position, Quaternion.identity);
                orb1.GetComponent<HealingOrb>().SetTarget(playerObj, 1);
                healing--;
            }
        }



        /*
        for (int i = 0; i < Mathf.RoundToInt(orbCount * power); i++)
        {
            GameObject orb1 = GameObject.Instantiate(orb, enemyObj.transform.position, Random.rotation);
            orb1.GetComponent<HealingOrb>().SetTarget(playerObj, healing);
        }
        */
    }
}
