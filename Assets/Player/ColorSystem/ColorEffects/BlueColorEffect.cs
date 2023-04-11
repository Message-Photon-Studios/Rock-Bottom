using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The blue color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BlueColorEffect")]
public class BlueColorEffect : ColorEffect
{
    [SerializeField] float slow;
    [SerializeField] float duration;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;
        enemy.ChangeSpeed(enemy.GetSpeed() / slow, duration*power);
        enemy.DamageEnemy(damage*power);
    }
}
