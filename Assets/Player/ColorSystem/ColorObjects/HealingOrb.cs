using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingOrb : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float launchSpeed;
    [SerializeField] float acceleration;
    private GameObject player;
    private Rigidbody2D body;
    private int healing;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        Vector2 direction = ((transform.position - player.transform.position) *Vector2.one).normalized;
        body.AddForce((new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)) + direction).normalized * launchSpeed);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 direction = ((player.transform.position - transform.position) * Vector2.one).normalized;
        body.AddForce(direction * speed * Time.fixedDeltaTime);
        body.drag += acceleration * Time.fixedDeltaTime;
        speed += speed*acceleration*Time.fixedDeltaTime; 
    }

    public void SetTarget(GameObject player, int healing)
    {
        this.player = player;
        this.healing = healing;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && healing > 0)
        {
            GetComponent<ParticleSystem>().Stop();
            player.GetComponent<PlayerStats>().HealPlayer(healing);
            healing = 0;
            Destroy(gameObject,0.5f);
        }
    }
}
