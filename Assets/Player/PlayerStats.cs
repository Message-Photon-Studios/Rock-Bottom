using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] float hitInvincibilityTime;
    [SerializeField] GameManager gameManager;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement movement;
    int maxHealth;
    float invincibilityTimer = 0;
    public int chanceToBlock = 0;

    public float colorNearbyRange = 0;
    public int chanceToColorNearby = 0;
    public float colorRainbowMaxedPower = 1;

    public int chanceThatEnemyDontMix = 0;

    [SerializeField] PlayerSounds playerSounds;

    [SerializeField] public float clockTime;
    [SerializeField] int clockDamage;
    float secTimer = 1;

    /// <summary>
    /// This event fires when the player health is changed. The float is the new health.
    /// </summary>
    public UnityAction<float> onHealthChanged;
    
    /// <summary>
    /// This event fires when the players max health is set or changed. The float is the new max health
    /// </summary>
    public UnityAction<float> onMaxHealthChanged;

    /// <summary>
    /// The player died
    /// </summary>
    public UnityAction onPlayerDied;

    private bool isDeathExecuted;
    public void Setup(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        maxHealth = health;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
    }

    void Update()
    {
        if(invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if(invincibilityTimer < 0)
            {
                invincibilityTimer = 0;
                Physics2D.IgnoreLayerCollision(3,6,false);
            }
                
        }

        if (gameManager && gameManager.allowsClockTimer)
        {
            clockTime -= Time.deltaTime;

            if (clockTime <= 0)
            {
                secTimer -= Time.deltaTime;
                if (secTimer <= 0)
                {
                    TickDamagePlayer(clockDamage);
                    secTimer = 1f;
                }
            }
        }
    }

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage)
    {
        if(invincibilityTimer > 0) return;
        //if(damage == 0) return;
        if (UnityEngine.Random.Range(0, 100) < chanceToBlock)
        {
            Debug.Log("Damage blocked");
            return;
        }
        Physics2D.IgnoreLayerCollision(3,6);
        health -= damage;
        invincibilityTimer = hitInvincibilityTime;
        GetComponent<PlayerCombatSystem>().RemoveAttackRoot();
        GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
        if(health <= 0)
        {
            PlayerReachZeroHp();
        }
        animator.SetTrigger("damaged");
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// For very small instances of damage over time. Does not add invincibility or amiation.
    /// This damage is unblockable.
    /// </summary>
    /// <param name="damage"></param>
    public void TickDamagePlayer(int damage)
    {
        if(damage <= 0) return;
        health-= damage;
        if(health <= 0)
        {
            PlayerReachZeroHp();
        }
        onHealthChanged?.Invoke(health);
    }

    private void PlayerReachZeroHp()
    {
        animator.SetBool("dead", true);
        movement.movementRoot.SetTotalRoot("dead", true);
        invincibilityTimer = 3f;
        playerSounds.PlayDeath();
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    /// <param name="healing"></param>
    public void HealPlayer (int healing) 
    {
        health += healing;
        if(health > maxHealth) health = maxHealth;
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Returns the players current health
    /// </summary>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns the players max health. 
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Adds health points to the players max health and also heals the player the same ammount
    /// </summary>
    /// <param name="addMaxHealth"></param>
    public void AddMaxHealth(int addMaxHealth)
    {
        maxHealth += addMaxHealth;
        health += addMaxHealth;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Kill the player
    /// </summary>
    public void KillPlayer()
    {
        if (isDeathExecuted)
            return;
        isDeathExecuted = true;
        //TODO
        Debug.Log("Player died. Player deaths not implemented");
        gameManager?.PlayerDied();
        onPlayerDied?.Invoke();
    }

    /// <summary>
    /// Returns true if the player is invincible
    /// </summary>
    /// <returns></returns>
    public bool IsInvincible()
    {
        return invincibilityTimer > 0;
    }

    /// <summary>
    /// Returns the clock time as a formated string in the format "min:sec"
    /// </summary>
    /// <returns></returns>
    public string GetClockTimeString()
    {
        int min = (int)clockTime/ 60;
        int sec = (int)clockTime % 60;
        return ((min < 0) ? "00" : (min < 10) ? "0" + min : min) + ":" + ((sec<0)?"00":(sec<10)?"0"+sec:sec);
    }
}
