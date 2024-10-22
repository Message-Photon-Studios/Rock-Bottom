using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] int maxShield = 50;
    [SerializeField] int shieldDecayIncrease = 1;
    [SerializeField] float hitInvincibilityTime;
    [SerializeField] LevelManager levelManager;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement movement;
    [SerializeField] GameObject blockAura;
    int maxHealth;
    float invincibilityTimer = 0;
    public int chanceToBlock = 0;

    public float colorNearbyRange = 0;
    public int chanceToColorNearby = 0;
    public float colorRainbowMaxedPower = 1;
    
    int shield = 0;
    int shieldDecay = 0;

    public int chanceThatEnemyDontMix = 0;

    [SerializeField] PlayerSounds playerSounds;

    float secTimer = 1;

    /// <summary>
    /// This event fires when the player health is changed. The float is the new health.
    /// </summary>
    public UnityAction<float> onHealthChanged;

    /// <summary>
    /// This event fires when the shield takes damage. The float is the new shield.
    /// </summary>
    public UnityAction<float> onShieldChanged;
    
    /// <summary>
    /// This event fires when the players max health is set or changed. The float is the new max health
    /// </summary>
    public UnityAction<float> onMaxHealthChanged;

    /// <summary>
    /// The player died
    /// </summary>
    public UnityAction onPlayerDied;

    private bool isDeathExecuted;

    private Dictionary<GameColor, float> colorArmour = new Dictionary<GameColor, float>();
    public void Setup(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        maxHealth = health;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
        colorArmour = new Dictionary<GameColor, float>();
    }
    void Update()
    {
        secTimer -= Time.deltaTime;
        if(secTimer <= 0)
        {
            secTimer = 1;
            //DO stuff each second here:

            if(shield > 0)
            {
                shield -= (shieldDecay<0)?0:shieldDecay;
                shieldDecay += shieldDecayIncrease;
                if(shield < 0) shield = 0;
                onShieldChanged?.Invoke(shield);
            }
        }

        if(invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
            Color tmp = GetComponent<SpriteRenderer>().color;
            tmp.a = 0.70f + Mathf.Cos(invincibilityTimer * MathF.PI * 6f)*0.15f;
            //if (tmp.a <= 0.5) tmp.a = 0.5f;
            //if (tmp.a >= 0.8) tmp.a = 0.8f;
            if (invincibilityTimer < 0)
            {
                invincibilityTimer = 0;
                tmp.a = 1;
                Physics2D.IgnoreLayerCollision(3,6,false);
            }
            GetComponent<SpriteRenderer>().color = tmp;
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
            GameObject aura = Instantiate(blockAura, transform);
            Destroy(aura, 1);
            Debug.Log("Damage blocked");
            //return;
        } else
        {
            if(shield >= damage)
            {
                shield -= damage;
                damage = 0;
                onShieldChanged?.Invoke(shield);
            } else if(shield > 0 && damage > shield)
            {
                damage -= shield;
                shield = 0;
                onShieldChanged?.Invoke(shield);
            }

            health -= damage;
            animator.SetTrigger("damaged");
        }
        Physics2D.IgnoreLayerCollision(3,6);
        
        invincibilityTimer = hitInvincibilityTime;
        GetComponent<PlayerCombatSystem>().RemoveAttackRoot();
        GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
        if(health <= 0)
        {
            PlayerReachZeroHp();
        }
        
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
    /// Adds shield to the player
    /// </summary>
    /// <param name="addShield"></param> 
    public void AddShield(int addShield)
    {
        shield += addShield;
        shieldDecay = 0;
        if(shield > maxShield) shield = maxShield;
        onShieldChanged?.Invoke(shield);
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
        //Debug.Log("Player died. Player deaths not implemented");
        levelManager?.PlayerDied();
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

    public float GetColorArmour (GameColor color)
    {
        if(colorArmour == null) return 0f;
        if(colorArmour.ContainsKey(color))
        {
            float armour = colorArmour[color];
            if(armour > .9f)
                return .9f;
            else
                return
                    armour;
        }
        else
            return 0f;
    }

    public void AddColorArmour(GameColor color, float addArmour)
    {
        if(colorArmour.ContainsKey(color))
            colorArmour[color] += addArmour;
        else
            colorArmour.Add(color, addArmour);
    }
}
