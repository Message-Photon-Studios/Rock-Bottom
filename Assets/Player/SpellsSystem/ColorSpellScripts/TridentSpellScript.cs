using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentSpellScript : SpellImpact
{
    [SerializeField] float strength;
    [SerializeField] LayerMask layersToHit;
    [SerializeField] float waitTime;
    GameObject player;
    Rigidbody2D body;
    int count = 0;
    float time;
    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        throw new System.NotImplementedException();
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
        if (body.velocity.sqrMagnitude <= 0.5f * ((count + 10f) / 10f))
        {
            if (count >= 20) Destroy(gameObject);
            Vector3 direction = player.transform.position - transform.position;
            float playerAngle = Mathf.Atan2(direction.y * spell.lookDir, direction.x * spell.lookDir) * Mathf.Rad2Deg ;
            Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * strength * ((count + 10f) / 10f));
            transform.rotation = newDir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, newDir * new Vector2(spell.lookDir, 0), 50f, layersToHit);
            if (hit.rigidbody && time < Time.time - waitTime)
            { 
                if (hit.rigidbody.gameObject == player)
                {
                    Vector2 dir = newDir * new Vector2(1 * spell.lookDir, 0);
                    body.velocity = (dir.normalized * 20 * (count + 10f)/10f);
                    count++;
                }
            }
        }
        else
        {
            time = Time.time;
        }
    }
}
