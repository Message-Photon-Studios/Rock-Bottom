using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldrichJoint : MonoBehaviour
{
    [SerializeField] float chance;
    [SerializeField] float minGravity;
    [SerializeField] float maxGravity;
    [SerializeField] float changeMultiplier;
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = Random.Range(minGravity, maxGravity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float random = Random.RandomRange(-1f, 1f);
        random *= changeMultiplier;
        body.gravityScale += random;
        if (body.gravityScale > maxGravity) body.gravityScale = maxGravity;
        if (body.gravityScale < minGravity) body.gravityScale = minGravity;
        //if (Random.Range(0,1) <= chance) body.gravityScale = Random.Range(minGravity, maxGravity) * gravityMultiplier;
    }
}
