using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] int maxShield = 50;
    [SerializeField] int maxPermanetShield = 20;
    [SerializeField] int shieldDecayIncrease = 1;
    [SerializeField] float hitInvincibilityTime;
    [SerializeField] LevelManager levelManager;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement movement;
    [SerializeField] GameObject blockAura;
    private ColorInventory colorInventory;
    int maxHealth;
    float invincibilityTimer = 0;
    public int chanceToBlock = 0;

    public float colorNearbyRange = 0;
    public int chanceToColorNearby = 0;
    public float colorRainbowMaxedPower = 1;
    
    int shield = 0;
    int shieldDecay = -1;

    public int chanceThatEnemyDontMix = 0;

    public int complimentaryDamage = 0;

    public bool corrosiveColor = false;

    [SerializeField] PlayerSounds playerSounds;

    float secTimer = 1;

    public Dictionary<string, int> itemVaribles;

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

    /// <summary>
    /// This event fires when the player is damaged. The enemy stats is null when the player is damaged by non enemies.
    /// </summary>
    public UnityAction<PlayerStats, EnemyStats> onPlayerDamaged;

    private bool isDeathExecuted;

    private Dictionary<GameColor, float> colorArmour = new Dictionary<GameColor, float>();
    private float adaptiveArmourBonus = 0f;
    private float defaultArmour = 0f;
    private float invincibilityBonus = 0f;

    #region Setup
    public void Setup(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        health += PermanentUpgradeManager.instance.upgrades.extraHealth;
        maxHealth = health;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
        colorArmour = new Dictionary<GameColor, float>();
        itemVaribles = new Dictionary<string, int>();
        colorInventory = GetComponent<ColorInventory>();
    }

    #endregion

    #region Update Loop
    void Update()
    {
        secTimer -= Time.deltaTime;
        if(secTimer <= 0)
        {
            secTimer = 1;
            //DO stuff each second here:

            if(shield > maxPermanetShield)
            {
                shield -= (shieldDecay<0)?0:shieldDecay;
                shieldDecay += shieldDecayIncrease;
                if(shield < maxPermanetShield) shield = maxPermanetShield;
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
                RemovePlayerInvincible();
            }
            GetComponent<SpriteRenderer>().color = tmp;
        }

        

    }

    #endregion

    #region Damage Player

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage, EnemyStats enemy)
    {
        if(invincibilityTimer > 0) return;
        if(enemy != null && damage > 0)
        {
            damage = Mathf.RoundToInt(damage * (1f - GetColorArmour(enemy.GetColor())));
            if (damage <= 0) damage = 1;
        }

        shieldDecay = 0;
        if (UnityEngine.Random.Range(0, 100) < chanceToBlock)
        {
            GameObject aura = Instantiate(blockAura, transform);
            Destroy(aura, 1);
        }
        else if (enemy != null && colorInventory.CheckRoutedSheild(enemy.GetColor()))
        {
            //TODO add proper block Sheild
            GameObject aura = Instantiate(blockAura, transform);
            Destroy(aura, 1);
        }
        else
        {
            if (shield >= damage)
            {
                shield -= damage;
                damage = 0;
                onShieldChanged?.Invoke(shield);
            }
            else if (shield > 0 && damage > shield)
            {
                damage -= shield;
                shield = 0;
                onShieldChanged?.Invoke(shield);
            }

            health -= damage;
            animator.SetTrigger("damaged");
        }
        SetPlayerInvincible();
        GetComponent<PlayerCombatSystem>().RemoveAttackRoot();
        GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
        if(health <= 0)
        {
            PlayerReachZeroHp();
        }
        
        onHealthChanged?.Invoke(health);
        onPlayerDamaged?.Invoke(this, enemy);
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

    #endregion

    #region Healing & Health

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
    /// Removes max health. Will only damage player when necessary
    /// </summary>
    /// <param name="removeMaxHealth"></param>
    public void RemoveMaxHealth(int removeMaxHealth)
    {
        int damagePlayer = removeMaxHealth - (maxHealth-health);
        if(damagePlayer > 0) DamagePlayer(damagePlayer, null);
        else DamagePlayer(0, null);
        maxHealth -= removeMaxHealth;

        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
    }

    #endregion

    #region Shield

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

        public int GetMaxShield()
    {
        return maxShield;
    }

    public int GetMaxPermanentShield()
    {
        return maxPermanetShield;
    }

    public int GetShield()
    {
        return shield;
    }

    #endregion

    #region Kill Player

    private void PlayerReachZeroHp()
    {
        animator.SetBool("dead", true);
        movement.movementRoot.SetTotalRoot("dead", true);
        invincibilityTimer = 3f;
        playerSounds.PlayDeath();
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
        EnemyStats.chaoticMixer = false; //Resets chaoticMixer 
        levelManager?.PlayerDied();
        onPlayerDied?.Invoke();
    }

    #endregion

    #region Invincibility 

    /// <summary>
    /// Returns true if the player is invincible
    /// </summary>
    /// <returns></returns>
    public bool IsInvincible()
    {
        return invincibilityTimer > 0;
    }

    public void AddInvincibilityBonus(float time)
    {
        invincibilityBonus += time;
    }

    public void SetPlayerInvincible()
    {
        //Physics2D.IgnoreLayerCollision(3,6);
        //Physics2D.IgnoreLayerCollision(3,13);
        Physics2D.IgnoreLayerCollision(3,2);
        invincibilityTimer = hitInvincibilityTime + invincibilityBonus;
    }

    public void RemovePlayerInvincible()
    {
        //Physics2D.IgnoreLayerCollision(3,6, false);
        //Physics2D.IgnoreLayerCollision(3,13, false);
        Physics2D.IgnoreLayerCollision(3,2, false);

        invincibilityTimer = 0;
    }

    #endregion

    #region Armour

    public float GetColorArmour(GameColor color)
    {
        if(color == null) return 0;
        float armour = defaultArmour;
        if (colorArmour.ContainsKey(color)) armour += colorArmour[color];
        if (color != null && colorInventory.CheckIfActiveColorMatches(color)) armour += adaptiveArmourBonus;
        if (armour > .9f)
        {
            return .9f;
        }
            
        else
            return
                armour;
    }

    public void AddColorArmour(GameColor color, float addArmour)
    {
        if(colorArmour.ContainsKey(color))
            colorArmour[color] += addArmour;
        else
            colorArmour.Add(color, addArmour);
    }

    public void AddAdaptiveArmour(float addArmour)
    {
        adaptiveArmourBonus += addArmour;
    }

    public void AddDefaultArmour(float addArmour)
    {
        defaultArmour += addArmour;
    }

    #endregion
}
