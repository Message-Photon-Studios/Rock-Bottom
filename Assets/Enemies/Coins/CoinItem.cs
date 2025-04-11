using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer sparkles;
    [SerializeField] float collectDistance;
    [SerializeField] float acceleration;
    [SerializeField] CircleCollider2D trigger;
    private Rigidbody2D body;
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        Material[] materials = Resources.LoadAll<Material>("BreathingBloom");
        sprite.material = materials[Random.Range(0, materials.Length)];
        sparkles.material = sprite.material;
        body = GetComponent<Rigidbody2D>();
        // Apply a force in a random upwards directio
        body.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 200);

        float scale = transform.localScale.x;
        transform.localScale = new Vector3(0, 0, 1);
        StartCoroutine(appear(scale));

        StartCoroutine(hoverAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    // Collision callback
    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player collides with the coin, add the coin's value to the player's score
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ItemInventory>().AddCoins(value);
            Destroy(gameObject);
        } 
    }
    */

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float dis = Vector2.Distance(other.ClosestPoint(transform.position), transform.position);
            Debug.Log(dis + " " + trigger.radius);

            if (dis <= collectDistance)
            {
                other.gameObject.GetComponent<ItemInventory>().AddCoins(value);
                Destroy(gameObject);
            }
            else
            {
                Vector2 direction = (other.ClosestPoint(transform.position) - (Vector2) this.transform.position).normalized;
                body.AddForce(direction * (trigger.radius*transform.localScale.x - dis) * acceleration);
            }


        }
    }


    IEnumerator appear(float size)
    {
        const float speed = 0.5f;
        for (float i = 0; i < speed; i += Time.deltaTime)
        {
            transform.localScale = new Vector3(i * size / speed, i * size / speed, 1);
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = new Vector3(size, size, 1);
    }
    private IEnumerator hoverAnimation()
    {
        float offset = Random.Range(0, 100);
        while (true)
        {
            sprite.transform.position = new Vector3(
                sprite.transform.position.x, 
                sprite.transform.position.y + Mathf.Sin(offset + Time.time * 5) * 0.004f, 
                sprite.transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }
}
