using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlockAura : MonoBehaviour
{
    private float time = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        Vector3 scale = transform.localScale;
        Color color = GetComponent<SpriteRenderer>().color;
        scale.x = Mathf.Sin(time * Mathf.PI * 4)/4 + 1.3f;
        scale.y = Mathf.Sin(time * Mathf.PI * 4)/-4 + 1.3f;
        color.a = time;
        transform.localScale = scale;
        GetComponent<SpriteRenderer>().color = color;
    }
}
