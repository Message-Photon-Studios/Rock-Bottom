using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The green color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/GreenColorEffect")]
public class GreenColorEffect : ColorEffect
{
    [SerializeField] int damageOverTime;
    [SerializeField] float damageReduction;
    [SerializeField] float time;
    [SerializeField] GameObject poisonOrb;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        
        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;

        float useTime = time*EffectFunction(power);
        float scaledDamageReduction = damageReduction*EffectFunction(power);

        main.duration = useTime;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, useTime+.5f);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;


        enemy.PoisonDamage(Mathf.RoundToInt(damageOverTime * power), scaledDamageReduction, useTime, poisonOrb);
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power));
    }
}
