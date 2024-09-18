using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFlame : MonoBehaviour
{
    [SerializeField] float maxForce;
    [SerializeField] float minForce;
    private HashSet<GameObject> burnQueue = new HashSet<GameObject>();

    private (int damage, float timer, float range, GameObject particles, GameObject floorParticles, int flames) burning;
    
    
    [HideInInspector] public int dir = 1;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(0.15f, 0.75f)*dir, 0.5f).normalized * Random.Range(maxForce, minForce));
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + .5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {     
        if (collision.gameObject.CompareTag("Enemy") && !burnQueue.Contains(collision.gameObject))
        {
            burnQueue.Add(collision.gameObject);
            collision.gameObject.GetComponent<EnemyStats>()?.BurnDamage(burning.damage, burning.timer, burning.range, burning.particles, burning.floorParticles, false, burning.flames);
        }
    }

    public void SetBurning(int damage, float timer, float range, GameObject particles, GameObject floorParticles, int flames)
    {
        burning.damage = damage;
        burning.timer = timer;
        burning.range = range;
        burning.particles = particles;
        burning.floorParticles = floorParticles;
        burning.flames = flames;
    }

    public void ClearBurnQueue()
    {
        burnQueue = new HashSet<GameObject>();
    }

    public void ApplyFire(GameObject enemy)
    {
        if (!burnQueue.Contains(enemy))
        {
            burnQueue.Add(enemy);
        }
    }
}
