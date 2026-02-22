using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentSpellScript : SpellImpact
{
    [SerializeField] float rotateStrength;
    [SerializeField] float boostStrength;
    [SerializeField] LayerMask layersToHit;
    [SerializeField] float waitTime;
    [SerializeField] float despawnDistance;
    [SerializeField] float maxDistance;
    [SerializeField] int maxBoosts;
    [SerializeField] float canBoostAgianDistance;
    GameObject player;
    Rigidbody2D body;
    int count = 0;
    float time;
    float timeSinceLastBoost;
    float emmission = 20;
    bool canBeDestroyed = false;
    bool canBoostAgain = false;
    bool justSpawned = true;
    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        player = spell.GetPlayerObj();
        body = this.GetComponent<Rigidbody2D>();
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (justSpawned)
        {
            if (body.velocity.sqrMagnitude > 0.6f) justSpawned = false;
            return;
        }
        float playerDistance = Vector2.Distance(player.transform.position, transform.position);
        if (playerDistance <= despawnDistance && canBeDestroyed)
        {
            Destroy(gameObject);
        }
        if (playerDistance < canBoostAgianDistance) canBoostAgain = true;
        if (body.velocity.sqrMagnitude <= 0.5f * ((count + 10f) / 10f) || playerDistance >= maxDistance && (timeSinceLastBoost <= Time.time || canBoostAgain))
        {
            body.velocity = new Vector2(0,0);
            foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>()) particle.emissionRate = 0;
            if (count >= maxBoosts) Destroy(gameObject);
            Vector3 direction = player.transform.position - transform.position;
            float playerAngle = Mathf.Atan2(direction.y * spell.lookDir, direction.x * spell.lookDir) * Mathf.Rad2Deg ;
            Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * rotateStrength * ((count + 10f) / 10f));
            transform.rotation = newDir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, newDir * new Vector2(spell.lookDir, 0), 2f, layersToHit);

            if (time < Time.time - waitTime)
            { 
                if (hit.rigidbody)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Vector2 dir = newDir * new Vector2(1 * spell.lookDir, 0);
                    body.velocity = (dir.normalized * boostStrength * (count + 10f)/10f);
                    count++;
                    timeSinceLastBoost = Time.time + .5f;
                    canBeDestroyed = true;
                    canBoostAgain = false;
                    foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>()) particle.emissionRate = emmission;
                }
            } 
        }
        else
        {
            time = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        body.velocity = new Vector2(0, 0);
    }
}
