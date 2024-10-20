using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The blue color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BlueColorEffect")]
public class BlueColorEffect : ColorEffect
{
    [SerializeField] float force;
    [SerializeField] float slow;
    [SerializeField] float duration;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer)
    {
        
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;

        float effect = EffectFunction(power);
        float scaledDuration = duration * effect;
        float scaledSlow = slow*effect;
        main.duration = scaledDuration;
        Destroy(instantiatedParticles, scaledDuration*1.1f);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;
        if(!enemy.IsKnockbackImune())
        {
            Vector3 pushPoint = (forcePerspectivePlayer)?playerObj.transform.position:new Vector3(impactPoint.x, impactPoint.y, enemy.transform.position.z);
            enemy.GetComponent<Rigidbody2D>()?.AddForce((enemy.transform.position-pushPoint).normalized * force);
        }
        enemy.ChangeDrag(scaledSlow+1, scaledDuration);
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power));
    }
}
