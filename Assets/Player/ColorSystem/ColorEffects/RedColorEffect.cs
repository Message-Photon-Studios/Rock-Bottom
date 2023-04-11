using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The red color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/RedColorEffect")]
public class RedColorEffect : ColorEffect
{
    [SerializeField] float healing;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats player = playerObj.GetComponent<PlayerStats>();

        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;

        enemy.DamageEnemy(damage*power);
        player.HealPlayer(healing*power);
    }
}
