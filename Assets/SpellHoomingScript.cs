using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHoomingScript : MonoBehaviour
{
    [SerializeField] float range = 1;
    [SerializeField] float speed = 1;
    [SerializeField] bool smartTarget;
    [SerializeField] bool targetClosest;
    [SerializeField] bool targetRandom;
    Rigidbody2D body;
    GameColor color;
    GameObject target;
    GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        color = GetComponent<ColorSpell>().GetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            target = SetTarget();
            return;
        }
        if (target.GetComponent<EnemyStats>().GetColor() != null) if (target.GetComponent<EnemyStats>().GetColor() == color && smartTarget) target = SetTarget();

        Vector2 direction = ((target.transform.position - transform.position) * Vector2.one).normalized;
        body.AddForce(direction * speed * Time.deltaTime);
    }

    public GameObject SetTarget()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (targetClosest) return SetClose();
        if (targetRandom) return SetRandom();
        if (smartTarget) return SetSmart();
        return SetClose();
    }

    public GameObject SetClose()
    {
        GameObject target = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance <= Mathf.Pow(range, 2))
            {
                if (target == null) target = enemy;
                else if (distance < (target.transform.position - transform.position).sqrMagnitude) target = enemy;
            }
        }
        return target;
    }

    public GameObject SetRandom()
    {
        GameObject target = null;
        List<GameObject> inRange = new List<GameObject>();
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance <= Mathf.Pow(range, 2)) inRange.Add(enemy);
        }

        if (inRange.Count == 0) return null;

        int rand = Random.Range(0, inRange.Count);
        Debug.Log("Number: " + rand + " Count: " + inRange.Count);

        target = inRange[rand];
        return target;
    }

    public GameObject SetSmart()
    {
        GameObject target = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance <= Mathf.Pow(range, 2))
            {
                EnemyStats stats = enemy.GetComponent<EnemyStats>();
                if (stats.GetColor() != null)
                {
                    if (stats.GetColor() == color) continue;
                }
                if (target == null) target = enemy;
                else if (distance < (target.transform.position - transform.position).sqrMagnitude) target = enemy;
            }
        }
        return target;
    }
}
