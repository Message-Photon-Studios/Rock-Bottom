using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purple color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/PurpleColorEffect")]
public class PurpleColorEffect : ColorEffect
{
    [SerializeField] float sleepTime;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;
        main.duration = sleepTime*2;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, sleepTime);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;
        enemy.DamageEnemy(damage);
        if(enemy != null) enemy.SleepEnemy(sleepTime);
    }
}
