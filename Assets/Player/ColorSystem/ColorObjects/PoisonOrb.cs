using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonOrb : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float launchSpeed;
    [SerializeField] Rigidbody2D body;
    [SerializeField] GameObject particles;

    int poisonDamage;
    float poisonTimer;
    float poisonDamageReduction;
    GameObject poisonOrbPrefab;
    Transform target;
    float deathTime = 2f;

    bool dead = false;
    void OnTriggerStay2D(Collider2D other)
    {
        if(other != null && other.CompareTag("Enemy"))
        {
            EnemyStats stats = other.GetComponent<EnemyStats>();
            if(stats && !(stats.GetColor() && stats.GetColor().name == "Green" && stats.GetColorAmmount() > 0))
            {
                if(target == null) target = other.transform;
                else if (transform && Vector2.Distance(transform.position, other.transform.position) < Vector2.Distance(transform.position, target.position))
                    target = other.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if(dead) return;
        if(target == null)
        {   
            deathTime-= Time.fixedDeltaTime;
            if(deathTime <= 0) Destroy(gameObject);
            return;
        }
        Vector2 direction = ((target.position - transform.position) * Vector2.one).normalized;
        body.AddForce(direction * speed * Time.fixedDeltaTime);
        body.drag += acceleration * Time.fixedDeltaTime;
        speed += speed*acceleration*Time.fixedDeltaTime; 

        if(Vector2.Distance(target.transform.position, transform.position) < .4f) 
        {
            EnemyStats stats = target.GetComponent<EnemyStats>();
            stats.PoisonDamage(poisonDamage, poisonDamageReduction, poisonTimer, poisonOrbPrefab);
            GameObject instantiatedParticles = GameObject.Instantiate(particles, target.position, target.rotation);
            var main = instantiatedParticles.GetComponent<ParticleSystem>().main;
                instantiatedParticles.GetComponent<ParticleSystem>().Play();
            Destroy(instantiatedParticles, poisonTimer+.5f);
            // Set enemy as parent of the particle system
            instantiatedParticles.transform.parent = target.transform;
            GetComponent<ParticleSystem>().Stop();
            Destroy(transform.GetChild(0).gameObject);
            dead = true;
            Destroy(gameObject,0.5f);
        }
    }

    public void SetupOrb(int damage, float damageReduction, float timer, GameObject poisonOrbPrefab)
    {
        poisonDamage = Mathf.RoundToInt(damage*1.5f);
        poisonDamageReduction = damageReduction;
        poisonTimer = timer-1;
        if(timer < 0)
        {
            Destroy(gameObject);
            return;
        }
        this.poisonOrbPrefab = poisonOrbPrefab;
        
        body.AddForce(new Vector2(Random.Range(-1f,1f), 1f).normalized * launchSpeed);
    }
}
