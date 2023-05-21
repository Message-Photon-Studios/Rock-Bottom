using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer sparkles;
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        Material[] materials = Resources.LoadAll<Material>("BreathingBloom");
        sprite.material = materials[Random.Range(0, materials.Length)];
        sparkles.material = sprite.material;
        // Apply a force in a random upwards direction
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), 300));

        float scale = transform.localScale.x;
        transform.localScale = new Vector3(0, 0, 1);
        StartCoroutine(appear(scale));

        StartCoroutine(hoverAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        while (true)
        {
            sprite.transform.position = new Vector3(
                sprite.transform.position.x, 
                sprite.transform.position.y + Mathf.Sin(Time.time * 2) * 0.002f, 
                sprite.transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }
}
