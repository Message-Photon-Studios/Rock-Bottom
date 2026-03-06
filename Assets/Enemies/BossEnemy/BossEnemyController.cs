using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class BossEnemyController : MonoBehaviour
{
    [SerializeField] GameColor[] bossColors;
    [SerializeField] float changeColorTime;
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject[] spawnEnemies;
    [SerializeField] BossHandController[] hands;
    [SerializeField] GameObject[] hunters;

    public static Action onBossDefeated;

    EnemyStats stats;
    float bossStartHealth;
    public int phase = 0;
    PlayerStats player;
    bool playerDied = false;
    float changeColorTimer;
    void Start()
    {
        stats = GetComponent<EnemyStats>();
        bossStartHealth = stats.GetHealth();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        stats.onEnemyDeath += BossDied;
        player.onPlayerDied += PlayerDied;
        stats.onDamageTaken += OnDamageTaken;

        foreach (BossHandController hand in hands)
        {
            hand.ChangeColor(stats.GetColor());
        }

        stats.onColorChanged += SetHandsColor;
    }

    void OnDisable()
    {
        stats.onEnemyDeath -= BossDied;
        player.onPlayerDied -= PlayerDied;
        stats.onDamageTaken -= OnDamageTaken;
        stats.onColorChanged -= SetHandsColor;
    }

    void SetHandsColor(GameColor color)
    {
        foreach (BossHandController hand in hands)
        {
            hand.ChangeColor(stats.GetColor());
        }

    }

    void OnDamageTaken(float damage, Vector2 atPosition)
    {
        float health = stats.GetHealth();

        int newPhase =  (int)(4f * (1f-(health/(float)bossStartHealth)));
        if(newPhase != phase)
        {
            phase = newPhase;
            NewPhase();
        }
    }

    void Update()
    {
        changeColorTimer -= Time.deltaTime;
        if(changeColorTimer <= 0)
        {
            stats.SetColor(bossColors[UnityEngine.Random.Range(0, bossColors.Length)], 4);
            changeColorTimer = changeColorTime;
        }
    }

    /// <summary>
    /// Controls the boss phases
    /// </summary>
    void NewPhase()
    {
        switch (phase)
        {
            case 0: break;
            case 1:
            {
                hunters[0].transform.position = transform.position + Vector3.right*-1.5f;
                hunters[0].SetActive(true);
                break;
            }
            case 2: {
                for (int i = 1; i < 3; i++)
                {
                    hunters[i].transform.position = transform.position + Vector3.right*(i%2*2-1) *1.5f;
                    hunters[i].SetActive(true);
                    AddMinionMax(1);
                } 
                break;
            }
            case 3:
            {
                for (int i = 3; i < 5; i++)
                {
                    hunters[i].transform.position = transform.position + Vector3.right*(i%2*2-1) *1.5f;
                    hunters[i].SetActive(true);
                } 
                AddMinionMax(2);
                break;
            }
            default: break;
        }
    }

    void AddMinionMax(int addMax)
    {
        GetComponent<BossEnemyMain>().IncreaseMinionAmount(addMax);
    }

    void BossDied(EnemyStats deadBoss)
    {
        GetComponent<BossEnemyMain>().KillAllMinions();
        for (int i = 0; i < hunters.Length; i++)
        {
            if(hunters[i] != null && hunters[i].gameObject.activeSelf)
                hunters[i].GetComponent<EnemyStats>().KillEnemy();
        }
        onBossDefeated?.Invoke();
        GameManager.instance?.onGameWon?.Invoke();
    }

    void PlayerDied()
    {
        healthBar.SetActive(false);
    }

    public GameObject WispSetupAndSpawnObj(GameObject wisp)
    {
        int i = UnityEngine.Random.Range(0, spawnEnemies.Length);
        return spawnEnemies[i];
    }
}
