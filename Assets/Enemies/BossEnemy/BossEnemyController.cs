using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyController : MonoBehaviour
{
    [SerializeField] float changeHandTime;
    [SerializeField] float handDeathTime;
    [SerializeField] List<Enemy> startingHands;
    [SerializeField] List<Enemy> secondPhaseHands;
    [SerializeField] GameColor[] bossColors;
    [SerializeField] float changeColorTime;
    [SerializeField] GameObject deathUnlock;

    EnemyStats stats;
    List<Enemy> hands = new List<Enemy>(0);
    int spellHand = 0;
    int idleHand = 0;
    float changeTimer = 0;
    float handDeathTimer;
    float bossStartHealth;
    bool secondPhase = false;
    Transform wispTarget = null;

    PlayerStats player;
    bool playerDied = false;
    float changeColorTimer;
    void Start()
    {
        AddHands(startingHands);
        changeTimer = Random.Range(0, changeHandTime);
        stats = GetComponent<EnemyStats>();
        bossStartHealth = stats.GetHealth();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        stats.onEnemyDeath += BossDied;
    }

    void Update()
    {
        if (player.GetHealth() <= 0 && !playerDied)
        {
            GetComponent<Enemy>().enabled = false;
            foreach(Enemy hand in hands)
            {
                hand.enabled = false;
            }
            playerDied = true;
            return;
        }
        changeTimer -= Time.deltaTime;
        if(changeTimer <= 0)
        {
            changeTimer = Random.Range(0, changeHandTime);
            ChangeSpellHand();
            if(hands.Count > 2)
            {
                ChangeIdleHand();
            }
        }

        if (handDeathTimer > 0)
        {
            handDeathTimer -= Time.deltaTime;
            if(handDeathTimer <= 0)
            {
                foreach (Enemy hand in hands)
                {
                    if(!hand.gameObject.activeSelf)
                    {
                        hand.gameObject.SetActive(true);
                    }
                }
            }
        }

        if(!secondPhase)
        {
            if(stats.GetHealth() < bossStartHealth/2)
            {
                foreach(Enemy hand in secondPhaseHands)
                {
                    hand.gameObject.SetActive(true);
                }
                secondPhase = true;
                AddHands(secondPhaseHands);
            }
        }

        changeColorTimer -= Time.deltaTime;
        if(changeColorTimer <= 0)
        {
            stats.SetColor(bossColors[Random.Range(0, bossColors.Length)], 4);
            changeColorTimer = changeColorTime;
        }
    }

    void BossDied()
    {
        foreach(Enemy hand in hands)
        {
            hand.GetComponent<EnemyStats>().KillEnemy();
        }

        deathUnlock.SetActive(true);
    }

    void ChangeSpellHand()
    {
        hands[spellHand].SetBoolFalse("spellMode");
        spellHand = Random.Range(0, hands.Count);
        hands[spellHand].SetBoolTrue("spellMode");
    }

    void ChangeIdleHand()
    {
        hands[idleHand].SetBoolFalse("idle");
        idleHand = Random.Range(0, hands.Count);
        if(idleHand == spellHand)
        {
            idleHand++;
            idleHand %= hands.Count;
        }

        hands[idleHand].SetBoolTrue("idle");
    }
    
    void AddHands(List<Enemy> handsAdded)
    {
        foreach(Enemy enemy in handsAdded)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            enemyStats.onDamageTaken += (float damageTaken, Vector2 tmp) => {HandTookDamage(damageTaken);};
        }

        hands.AddRange(handsAdded);
    }

    void HandTookDamage(float damageTaken)
    {
        stats.DamageEnemy((int)damageTaken);
    }



    public void WispSpawned(GameObject wisp)
    {
        int i = Random.Range(0, hands.Count);
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if(i != hands.Count)
        {
            target = hands[i].gameObject;
            wisp.GetComponent<Wisp>().SetTarget(target);
        }
    }
}
