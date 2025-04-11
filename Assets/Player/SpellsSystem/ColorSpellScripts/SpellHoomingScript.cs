using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHoomingScript : MonoBehaviour
{
    [SerializeField] float range = 1;
    [SerializeField] float speed = 1;
    [SerializeField] float spawnTargetDelay = 0;
    [SerializeField] HoomingSettings settings;
    Rigidbody2D body;
    GameColor color;
    GameObject target;
    GameObject[] enemies;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        color = GetComponent<ColorSpell>().GetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= spawnTargetDelay)
        {
            timer += Time.deltaTime;
            return;
        }
        if (target == null)
        {
            target = SetTarget();
            if (target == null) return;
        }
        if (target.GetComponent<EnemyStats>().GetColor() != null) if (target.GetComponent<EnemyStats>().GetColor() == color) target = SetTarget();

        Vector2 direction = ((target.transform.position - transform.position) * Vector2.one).normalized;
        body.AddForce(direction * speed * Time.deltaTime);
    }

    public GameObject SetTarget()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        switch (settings)
        {

            case HoomingSettings.SMART_CLOSEST: 
                GameObject target = SetSmart();
                if (target == null) return SetClose();
                return target;
            case HoomingSettings.SMART_RANDOM:
                return SetRandom();
            default:
                return SetClose();
        }
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
        bool canSmartTarget = false;
        PlayerStats playerStats = PlayerLevelMananger.instance.playerStats;
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance <= Mathf.Pow(range, 2)) 
            {
                if (!canSmartTarget)
                {
                    EnemyStats stats = enemy.GetComponent<EnemyStats>();
                    if (stats.GetColor() != null) if (stats.GetColor() != color || playerStats.corrosiveColor) canSmartTarget = true;
                    if (stats.GetColor() == null) canSmartTarget = true;
                }
                inRange.Add(enemy);
            }
        }

        if (inRange.Count == 0) return null;
        int rand = Random.Range(0, inRange.Count);
        target = inRange[rand];
        if (canSmartTarget)
        {
            while(target.GetComponent<EnemyStats>().GetColor() == color)
            {
                inRange.RemoveAt(rand);
                if (inRange.Count == 0) return null;
                rand = Random.Range(0, inRange.Count);
                target = inRange[rand];
            }
        }

        
        return target;
    }

    public GameObject SetSmart()
    {
        GameObject target = null;
        PlayerStats playerStats = PlayerLevelMananger.instance.playerStats;
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance <= Mathf.Pow(range, 2))
            {
                EnemyStats stats = enemy.GetComponent<EnemyStats>();
                if (stats.GetColor() != null)
                {
                    if (stats.GetColor() == color && !playerStats.corrosiveColor) continue;
                }
                if (target == null) target = enemy;
                else if (distance < (target.transform.position - transform.position).sqrMagnitude) target = enemy;
            }
        }
        return target;
    }

    [System.Serializable]
    public enum HoomingSettings
    {
        SMART_CLOSEST, SMART_RANDOM, CLOSEST
    } 
}
