using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The red color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/RedColorEffect")]
public class RedColorEffect : ColorEffect
{
    [SerializeField] int healing;
    [SerializeField] float force;
    [SerializeField] GameObject orb;
    [SerializeField] int orbCount;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats player = playerObj.GetComponent<PlayerStats>();

        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;

        if (!enemy.IsKnockbackImune())
            enemy?.GetComponent<Rigidbody2D>()?.AddForce((player.transform.position- enemy.transform.position).normalized * force);
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power));
        //player.HealPlayer(Mathf.RoundToInt(healing*power));


        for (int i = 0; i < Mathf.RoundToInt(orbCount * power); i++)
        {
            GameObject orb1 = GameObject.Instantiate(orb, enemyObj.transform.position, Random.rotation);
            orb1.GetComponent<HealingOrb>().SetTarget(playerObj, healing);
        }
        
    }
    public override string UpdateDesc(float power)
    {
        return " Hit: " + Mathf.RoundToInt(damage*power) + "\n Healing Orbs: " + Mathf.RoundToInt(orbCount*power);
    }
}
