using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBeamScript : MonoBehaviour
{
    [SerializeField] float range = 10;
    [SerializeField] GameObject hitShape;
    [SerializeField] ParticleSystem rootParticle;
    // Start is called before the first frame update
    void Start()
    {
        int dir = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().lookDir;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(dir, 0), range, GameManager.instance.maskLibrary.onlySolidGround());
        float hitRange;
        if (hit.distance == 0)
        {
            hitRange = range;
        } else
        {
            hitRange = hit.distance;
        }
        GetComponent<SpriteRenderer>().size = new Vector2(hitRange, 0.9f);
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        collider.size = new Vector2(hitRange, collider.size.y);
        Vector2 hitVector = new Vector2(hitRange / 2 * dir, 0);
        transform.position = new Vector3(transform.position.x + hitVector.x, transform.position.y);
        hitShape.transform.position = new Vector2(hitShape.transform.position.x + hitVector.x, hitShape.transform.position.y);
        ParticleSystem.ShapeModule rootShape = rootParticle.shape;
        rootShape.position = hitVector * -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
