using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOrb : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float launchSpeed;
    [SerializeField] float acceleration;
    private GameObject player;
    private Rigidbody2D body;
    private ParticleSystem particle;
    private int colorAmount;
    private GameColor color;
    
    // Start is called before the first frame update
    void Init()
    {
        body = GetComponent<Rigidbody2D>();
        particle = GetComponent<ParticleSystem>();
        Vector2 direction = ((transform.position - player.transform.position) *Vector2.one).normalized;
        body.AddForce((new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)) + direction).normalized * launchSpeed);
        if (colorAmount >= 4) particle.startSize = 0.2f;
        else if (colorAmount >= 2) particle.startSize= 0.1f;
        var pfxMain = particle.main;
        pfxMain.startColor = new ParticleSystem.MinMaxGradient(color.plainColor);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 direction = ((player.transform.position - transform.position) * Vector2.one).normalized;
        body.AddForce(direction * speed * Time.fixedDeltaTime);
        body.drag += acceleration * Time.fixedDeltaTime;
        speed += speed*acceleration*Time.fixedDeltaTime; 
    }

    public void SetTarget(GameObject player, int colorAmount, GameColor gameColor)
    {
        this.color = gameColor;
        this.player = player;
        this.colorAmount = colorAmount;
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && colorAmount > 0)
        {
            GetComponent<ParticleSystem>().Stop();
            player.GetComponent<ColorInventory>().AddColorOrbColor(color, colorAmount);
            colorAmount = 0;
            Destroy(gameObject,0.5f);
        }
    }
}
