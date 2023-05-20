using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The green color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/GreenColorEffect")]
public class GreenColorEffect : ColorEffect
{
    [SerializeField] float damageOverTime;
    [SerializeField] float time;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        enemy.DamageEnemy(damage*power);
        
        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;

        float useTime = time;
        if(enemy.GetHealth() <= damageOverTime*power*time)
        {
            useTime = enemy.GetHealth()/damageOverTime*power-1;
        }

        main.duration = useTime;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, useTime*1.2f);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;


        enemy.PoisonDamage(damageOverTime * power, useTime);
    }
}
