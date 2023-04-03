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
        instantiatedParticles.GetComponent<ParticleSystem>().time = sleepTime*2;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, sleepTime);
        enemy.DamageEnemy(damage);
        if(enemy != null) enemy.SleepEnemy(sleepTime);
    }
}
