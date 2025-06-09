using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleLightSpawner : MonoBehaviour
{
    [SerializeField] Light2D lightPrefab;

    List<ParticleCollisionEvent> collisionEvents;
    void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        int numOfCollisions = part.GetCollisionEvents(this.gameObject, collisionEvents);
        foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
        {
            Debug.Log(collisionEvent);
            GameObject lightSpawn = Instantiate(lightPrefab.gameObject, collisionEvent.intersection, transform.rotation);
            lightSpawn.GetComponent<Light2D>().color = GetComponent<ParticleSystem>().main.startColor.color;
            Destroy(lightSpawn, 3f);
        }
    }
}
