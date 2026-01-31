using System;
using System.Collections;
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
    [SerializeField] float shieldDecayIncrease = 1;
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
    public int rainbowedDamage = 0;
    public float rainbowExecutePercentage = .2f;
    
    int shield = 0;
    float shieldDecay = -1;

    public int complimentaryDamage = 0;

    public bool corrosiveColor = false;

    private List<EnemyStats> RedList = new List<EnemyStats>();

    [SerializeField] PlayerSounds playerSounds;

    float secTimer = 1;

    public Dictionary<string, int> itemVaribles;

    /// <summary>
    /// This event fires when the player health is changed. The float is the new health.
    /// </summary>
    public UnityAction<float, GameColor> onHealthChanged;

    /// <summary>
    /// This event fires when the shield takes damage. The float is the new shield.
    /// </summary>
    public UnityAction<float> onShieldChanged;

    public UnityAction<float> onMaxShieldChanged;

    public UnityAction<float> onMaxPermanentShieldChanged;
    
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

    /// <summary>
    /// This event fires when the player is damaged and loses HP. The enemy stats is null when the player is damaged by non enemies.
    /// </summary>
    public UnityAction<PlayerStats, EnemyStats> onPlayerRealDamage;

    private bool isDeathExecuted;

    private Dictionary<GameColor, float> colorArmour = new Dictionary<GameColor, float>();
    private float adaptiveArmourBonus = 0f;
    private float defaultArmour = 0f;
    private float invincibilityBonus = 0f;
    private List<InkArmorScript> inkArmorList = new List<InkArmorScript>();

    private const string lifelineName = "Lifeline";

    public bool isDead { get; private set; } = false;

    #region Setup
    public void Setup(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        colorArmour = new Dictionary<GameColor, float>();
        itemVaribles = new Dictionary<string, int>();
        colorInventory = GetComponent<ColorInventory>();
    }

    void Start()
    {
        health += PermanentUpgradeManager.instance.upgrades.extraHealth;
        maxHealth = health;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health, null);
        isDead = false;
    }

    #endregion

    #region Update Loop
    float drainTimer = 0;
    void Update()
    {
        secTimer -= Time.deltaTime;
        if (secTimer <= 0)
        {
            secTimer = 1;
            //DO stuff each second here:

            if (shield > maxPermanetShield)
            {
                Debug.Log(maxPermanetShield);
                shield -= Mathf.RoundToInt((shieldDecay < 0) ? 0 : shieldDecay);
                shieldDecay += shieldDecayIncrease;
                if (shield < maxPermanetShield) shield = maxPermanetShield;
                onShieldChanged?.Invoke(shield);
            }
            if (colorInventory.crackedUrn)
            {
                drainTimer++;
                if (drainTimer >= 2f)
                {
                    drainTimer = 0;
                    colorInventory.DrainAllSlots(1);
                }
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

    public void DamagePlayer(int damage, EnemyStats enemy)
    {
        GameColor damageColor = null;
        if (enemy) damageColor = enemy.GetColor();
        DamagePlayer(damage, enemy, damageColor);
    }

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage, EnemyStats enemy, GameColor colorDamage)
    {
        if(invincibilityTimer > 0) return;
        damage = Mathf.CeilToInt(damage / 10) * 10; //splits the damage into 10hp chunks
        if(colorDamage && damage > 0)
        {
            damage = Mathf.RoundToInt(damage * (1f - GetColorArmour(colorDamage)));
            if (damage <= 0) damage = 1;
        }
        damage = Mathf.CeilToInt(damage/5)*5; //If the armor deduced the damage, divide into 5hp chunks

        DealRedListDamage(damage);
        shieldDecay = 0;
        EnemyStats enemySource = enemy;
        if (enemy) enemySource = enemy.GetParent();

        if (HasInkArmor(enemySource))
        {
            inkArmorList.Clear();
        }
        else if (UnityEngine.Random.Range(0, 100) < chanceToBlock)
        {
            GameObject aura = Instantiate(blockAura, transform);
            Destroy(aura, 1);
        }
        else if (colorInventory.CheckRoutedSheild(colorDamage))
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
            damage = Mathf.FloorToInt(damage / 5) * 5; //makes sure that the hp is divided into 5hp chunks
            health -= damage;
            if (damage > 0 && health > 0) onPlayerRealDamage?.Invoke(this, enemy);
            animator.SetTrigger("damaged");
        }
        SetPlayerInvincibleHit();
        GetComponent<PlayerCombatSystem>().RemoveAttackRoot();
        GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
        if(health <= 0)
        {
            PlayerReachZeroHp();
        }
        
        onHealthChanged?.Invoke(health, colorDamage);
        onPlayerDamaged?.Invoke(this, enemySource);
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
        onHealthChanged?.Invoke(health, null);
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
        onHealthChanged?.Invoke(health, null);
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
        onHealthChanged?.Invoke(health, null);
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
        onHealthChanged?.Invoke(health, null);
    }

    public void AddMaxShield(int addMaxShield)
    {
        maxShield += addMaxShield;
        onMaxShieldChanged?.Invoke(maxShield);
    }

    public void AddMaxPermanentShield(int addMaxPTHp)
    {
        maxPermanetShield += addMaxPTHp;
        onMaxPermanentShieldChanged?.Invoke(maxPermanetShield);
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
        if (Player.instance.playerInventory.HasItemWithName(lifelineName))
        {
            Player.instance.playerInventory.RemoveItemWithName(lifelineName);
            health = 0;
            HealPlayer(10);
            return;
        }

        isDead = true;
        invincibilityTimer = 3f;
        movement.movementRoot.SetTotalRoot("dead", true);
        StartCoroutine(DeathPause());
    }

    IEnumerator DeathPause()
    {
        CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
        cameraMovement.TeleportCamarera(transform.position);
        cameraMovement.ZoomCamera(1.8f, .2f);
        yield return new WaitForSeconds(.5f);
        animator.SetBool("dead", true);
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

    public void SetPlayerInvincibleHit()
    {
        SetPlayerInvincible(hitInvincibilityTime + invincibilityBonus);
    }

    public void SetPlayerInvincible(float time)
    {
        SetPlayerInvincible();
        invincibilityTimer = time;
    }

    public void SetPlayerInvincible()
    {
        invincibilityTimer = 10f;
        //Physics2D.IgnoreLayerCollision(3,6);
        //Physics2D.IgnoreLayerCollision(3,13);
        //Physics2D.IgnoreLayerCollision(3,2);
    }

    public void RemovePlayerInvincible()
    {
        //Physics2D.IgnoreLayerCollision(3,6, false);
        //Physics2D.IgnoreLayerCollision(3,13, false);
        //Physics2D.IgnoreLayerCollision(3,2, false);

        invincibilityTimer = 0;
    }

    #endregion

    #region Red Damage Effect

    public void AddEnemyToRedList(EnemyStats enemy)
    {
        if (!RedList.Contains(enemy))
        {
            RedList.Add(enemy);
        }
    }

    public void RemoveEnemyFromRedList(EnemyStats enemy)
    {
        RedList.Remove(enemy);
    }

    public void DealRedListDamage(int damage)
    {
        foreach(EnemyStats enemy in RedList.ToArray())
        {
            if (enemy == null)
            {
                RedList.Remove(enemy);
                continue;
            }
            enemy.DoRedDamage(damage);
        }
    }

    #endregion

    #region Armour

    public float GetColorArmour(GameColor color)
    {
        if(color == null) return 0;
        float armour = defaultArmour;
        if (colorArmour.ContainsKey(color)) armour += colorArmour[color];
        //if (color != null && colorInventory.CheckIfActiveColorMatches(color)) armour += adaptiveArmourBonus;
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

    #region Stored Spells

    public void AddInkArmor(InkArmorScript inkArmor)
    {
        inkArmorList.Add(inkArmor);
    }

    public void RemoveInkArmor(InkArmorScript inkArmor)
    {
        if(inkArmorList.Contains(inkArmor)) inkArmorList.Remove(inkArmor);
    }

    public bool HasInkArmor(EnemyStats enemy)
    {
        bool status = false;
           foreach(InkArmorScript inkArmor in inkArmorList.ToArray())
        {
            if (inkArmor == null) inkArmorList.Remove(inkArmor);
            else
            {
                status = true;
                inkArmor.InitiateArmor(enemy);
            }
        }
        return status;
    }

    #endregion
}
