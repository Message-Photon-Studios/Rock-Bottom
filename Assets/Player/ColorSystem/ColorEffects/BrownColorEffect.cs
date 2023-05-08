using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The brown color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BrownColorEffect")]
public class BrownColorEffect : ColorEffect
{
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        if(enemy.GetColor() == null)
        {
            enemy.DamageEnemy(0);
            return;
        }

        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;
        enemy.DamageEnemy(damage*power);
        enemy.AbsorbColor();
    }
}
