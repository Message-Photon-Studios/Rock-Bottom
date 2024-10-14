using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPlatformBehaviourScript : MonoBehaviour
{
    private Rigidbody2D body;

    // Start is called before the first frame update
    void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (body.velocity.y < 0)
        {
            gameObject.layer = GameManager.instance.maskLibrary.spellHitPlatfromLayer;
        } else
        {
            gameObject.layer = GameManager.instance.maskLibrary.spellLayer;
        }
    }
}
