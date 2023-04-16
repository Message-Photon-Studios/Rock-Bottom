using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/YellowColorEffect")]
public class YellowColorEffect : ColorEffect
{
    [SerializeField] float effectRange;
    [SerializeField] float force;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> affected = new List<GameObject>();
        foreach (GameObject obj in objs)
        {
            if(obj == null) continue;
            if((obj.transform.position - enemyObj.transform.position).sqrMagnitude < Mathf.Pow(effectRange*power,2))
            {
                AffectObject(obj);
            }
        }

        for (int i = 0; i < affected.Count; i++)
        {
            foreach (GameObject obj in objs)
            {
                if(obj == null) continue;
                if(affected[i] == null) continue;
                if((obj.transform.position - affected[i].transform.position).sqrMagnitude < Mathf.Pow(effectRange*power,2))
                {
                    AffectObject(obj);
                }
            }

        }


        void AffectObject(GameObject obj)
        {
            if(affected.Contains(obj)) return;
            GameObject instantiatedParticles = GameObject.Instantiate(particles, obj.transform.position, obj.transform.rotation);
            Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
            instantiatedParticles.GetComponent<ParticleSystem>().Play();
            // Set enemy as parent of the particle system
            instantiatedParticles.transform.parent = enemyObj.transform;
            affected.Add(obj);
            obj?.GetComponent<Rigidbody2D>()?.AddForce((enemyObj.transform.position - obj.transform.position).normalized * force);
            obj.GetComponent<EnemyStats>().DamageEnemy(damage*power);
        }
    }
}
