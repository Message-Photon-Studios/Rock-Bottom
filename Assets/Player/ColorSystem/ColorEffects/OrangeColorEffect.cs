using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/OrangeColorEffect")]
public class OrangeColorEffect : ColorEffect
{
    [SerializeField] float damageOverTime;
    [SerializeField] float time;
    [SerializeField] float effectRange;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objs)
        {
            if((obj.transform.position - enemyObj.transform.position).sqrMagnitude < effectRange)
            {
                GameObject instantiatedParticles = GameObject.Instantiate(particles, obj.transform.position, obj.transform.rotation);
                instantiatedParticles.GetComponent<ParticleSystem>().time = time*2;
                instantiatedParticles.GetComponent<ParticleSystem>().Play();
                Destroy(instantiatedParticles, time);

                obj.GetComponent<EnemyStats>().DamageOverTime(damageOverTime*power, time);

            }
        }

        enemyObj.GetComponent<EnemyStats>().DamageEnemy(damage*power);
    }
}
