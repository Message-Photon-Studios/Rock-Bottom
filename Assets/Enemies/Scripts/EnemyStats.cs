using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

/// <summary>
/// Important stats for an enemy.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class EnemyStats : MonoBehaviour
{
    [SerializeField] int health; //The health of the enemy
    [SerializeField] GameColor color; //The colorMat of the enemy
    [SerializeField] int colorAmmount; //The ammount of colorMat you will get when absorbing the colorMat from the enemy'
    [SerializeField] float movementSpeed; //The current movement speed of the enemy
    [SerializeField] CoinRange coinsDropped; //Keeps track of how much coins this enemy drops upon death

    private Collider2D myCollider;
    [SerializeField] private Material defaultColor; //The material that is used when there is no GameColor attached
    [SerializeField] private GameObject comboParticles;

    [SerializeField] private bool knockbackImune = false;
    [SerializeField] private float sleepForcedown; //The force downwards that will be applied to a sleeping enemy

    [SerializeField] private bool setColorByHand;

    [SerializeField] private float deathTimer = 0;
    

    private bool hasDeathTimer = false;

    /// <summary>
    /// The direction that the enemy is looking
    /// </summary>
    public float lookDir = -1;

    [SerializeField] GameObject colorOrbPrefab;

    private float normalMovementDrag; //The normal movement drag of the enemy
    private float movementSpeedTimer;
    private float normalAnimationSpeed;
    private float colorComboTimer = 4.5f; //The timer before the enemy explode

    bool enemySleep = false; //If the enemy sleep is true the enemy will be inactive
    private float sleepTimer = 0;
    private float lastSleep = 0;
    private float sleepPowerBonus = 0f; //The extra damage dealt to a slept enemy
    GameObject sleepParticles;
    [HideInInspector] public int currentCombo = 0; //At what stage this combo is at

    private float secTimer = 0f; //Makes sure that all timers are updated only each second. 

    private Animator animator;

    [HideInInspector] public EnemySounds enemySounds;

    int poisonDamageToTake;
    private float poisonDamageReduction = 0;
    private float poisonTimer = 0;
    GameObject poisonOrbPrefab;

    [HideInInspector] public float spawnPower = 1f;

    private (int damage, float timer, float range, GameObject particles, GameObject[] burnable, GameObject floorParticles, bool mustBurn, GameObject enemyParticles, int flames) burning;
    /// <summary>
    /// This event fires when the enemys health is changed. The float is the damage received.
    /// </summary>
    public UnityAction<float> onHealthChanged;
    public UnityAction<float, Vector2> onDamageTaken;
    public UnityAction<GameColor> onColorChanged;

    public UnityAction onEnemySlept;

    /// <summary>
    /// The enemy died
    /// </summary>
    public UnityAction onEnemyDeath;

    private bool enemyDead = false;
    [CanBeNull] private Coroutine currentCoroutine;

    public bool isColoredThisFrame {get; private set;} = false;
    GameObject player;
    PlayerStats playerStats;
    PlayerCombatSystem playerCombat;
    #region Setup
    void Awake()
    {
        normalMovementDrag = GetComponent<Rigidbody2D>().drag;
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        normalAnimationSpeed = animator.speed;
    }

    void Start()
    {
        if(!setColorByHand) //Moved form awake
            color = LevelManager.instance.GetComponent<EnemyManager>().GetRandomEnemyColor();
        if(color != null)
            GetComponent<SpriteRenderer>().material = color.colorMat;
        else
            GetComponent<SpriteRenderer>().material = defaultColor;

        onDamageTaken += DmgNumber.create;
        onEnemyDeath += () => dropCoins(coinsDropped.GetReward());
        enemySounds = GetComponent<EnemySounds>();
        onColorChanged?.Invoke(color);
        if (deathTimer > 0) hasDeathTimer = true;
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerCombat = player.GetComponent<PlayerCombatSystem>();
    }

    void OnValidate()
    {
        myCollider = GetComponent<Collider2D>();
        if(color != null)
            GetComponent<SpriteRenderer>().material = color.colorMat;
        else
            GetComponent<SpriteRenderer>().material = defaultColor;
    }

    void OnDisable()
    {
        CleanEffects();
    }

    /// <summary>
    /// Removes all effects on enemy
    /// </summary>
    void CleanEffects()
    {
        animator.SetBool("sleep", false);
        WakeEnemy();
        GetComponent<Rigidbody2D>().drag = normalMovementDrag;
        poisonDamageToTake = 0;
        poisonDamageReduction = 0;
        poisonTimer = 0;
        StopBurning();
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_takingDmg", 0);
    }

    #endregion

    #region Update
    void Update()
    {
        if(isColoredThisFrame) isColoredThisFrame = false;
        if(secTimer > 1f)
        {
            if(color != null && color.name == "Rainbow")
            {
                int rainbowDamage = (int)(playerCombat.rainbowComboDamage*playerStats.colorRainbowMaxedPower);

                if(rainbowDamage >= health)
                {
                    DealRainbowDamage(rainbowDamage);
                }

                colorComboTimer--;

                if (colorComboTimer <= 0)
                {
                    DealRainbowDamage(rainbowDamage);
                }
            }

            if(movementSpeedTimer > 0)
            {
                movementSpeedTimer --;
                if(movementSpeedTimer <= 0)
                {
                    movementSpeedTimer = 0;
                    GetComponent<Rigidbody2D>().drag = normalMovementDrag;
                    animator.speed = normalAnimationSpeed;
                }
            }

            if(sleepTimer > 0)
            {
                sleepTimer --;
                if(sleepTimer <= 0)
                {
                    sleepTimer = 0;
                    WakeEnemyAnimation();
                }
            }

            if(poisonTimer > 0)
            {   
                DamageEnemy(poisonDamageToTake);
                poisonTimer --;
                if(poisonTimer <= 0)
                {
                    poisonDamageToTake = 0;
                    poisonDamageReduction = 0;
                }
            }

            if(burning.damage > 0 && burning.timer > 0)
            {
                if (burning.mustBurn || GetColor() == null || !GetColor().name.Equals("Orange")) DamageEnemy(burning.damage);
                //if(color?.name != "Orange" || color == null) DamageEnemy(burning.damage);
                //else DamageEnemy(0);
                float timer = burning.timer;
                timer --;
                burning.timer = timer;

                int damage = burning.damage;
                damage-=4;
                burning.damage = damage;

                //Debug.Log("burning: d " + burning.damage + " : t " + burning.timer);

                if(burning.damage <= 0 || burning.timer <= 0)
                {
                    StopBurning();
                    return;
                }
                

                foreach(GameObject obj in burning.burnable)
                {
                    if(obj == null) continue;
                    float dist = Vector2.Distance(transform.position, obj.transform.position);
                    if(dist < burning.range)
                    {
                        obj.GetComponent<EnemyStats>()?.BurnDamage(burning.damage+6, burning.timer+2, burning.range, burning.particles, burning.floorParticles, false, burning.flames);
                    }
                }
            }

            if(hasDeathTimer)
            {
                deathTimer--;
                if(deathTimer <= 0) KillEnemy();
            }
            secTimer = 0f;

        }
        secTimer += Time.deltaTime;

        if(IsAsleep())
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.down*sleepForcedown*Time.deltaTime);
        }
    }

    #endregion

    #region Damaged and Kill
    /// <summary>
    /// Damage the enemy with the specified damage
    /// </summary>
    /// <param name="damage"></param>
    public void DamageEnemy(int damage)
    {
        //if (enemySleep) WakeEnemyAnimation();

        health -= damage;

        onHealthChanged?.Invoke(health);
        onDamageTaken?.Invoke(damage, transform.position);
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        if(health <= 0) KillEnemy();
        else currentCoroutine = StartCoroutine(dmgResponse());
    }

    public IEnumerator dmgResponse()
    {
        Material mat = GetComponent<SpriteRenderer>().material;

        mat.SetFloat("_takingDmg", 1);
        yield return new WaitForSeconds(0.1f);
        mat.SetFloat("_takingDmg", 0);
    }
    
    /// <summary>
    /// Kill then enemy
    /// </summary>
    public void KillEnemy()
    {
        if(isPoisoned())
        {
            GameObject orb = GameObject.Instantiate(poisonOrbPrefab,transform.position, Quaternion.identity) as GameObject;
            orb.GetComponent<PoisonOrb>().SetupOrb(poisonDamageToTake, poisonDamageReduction, poisonTimer, poisonOrbPrefab);
            poisonTimer = 0;
            poisonDamageToTake = 0;
            poisonDamageReduction = 0;
        }
        GetComponent<Rigidbody2D>().drag = normalMovementDrag;
        animator.speed = normalAnimationSpeed;
        enemyDead = true;
        enemySounds?.PlayDeath();
        //TODO
        if (animator.GetBool("dead")) return;
        Debug.Log(gameObject.name + " died");
        animator.SetBool("dead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5);
        SleepEnemy(10, 1, null);
        int drainAmount = 0;
        if(color != null && color.name.Equals("Rainbow") && colorOrbPrefab != null && colorAmmount-drainAmount > 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                GameObject orb = Instantiate(colorOrbPrefab, GetPosition(), transform.rotation) as GameObject;  
                orb.GetComponent<ColorOrb>().SetTarget(player, colorAmmount - drainAmount, color);
            }
        }
        onEnemyDeath?.Invoke();
    }

    public bool IsDead()
    {
        return enemyDead;
    }

    public void DropCoins()
    {

    }

    /// <summary>
    /// Delets the enemy
    /// </summary>
    public void DeleteEnemy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Returns the enemys current health
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }

    public bool isFrozen()
    {
        return movementSpeedTimer > 0;
    }

    #endregion

    #region Rainbow damage

    private void DealRainbowDamage(int rainbowDamage)
    {
        GameManager.instance.tipsManager.DisplayTips("rainbowCombo");
        DamageEnemy(rainbowDamage);
        AbsorbColor();
        colorComboTimer = 2f;

        GameObject instantiatedParticles = GameObject.Instantiate(comboParticles, transform.position, transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, 1f);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = transform;
    }

    #endregion
    
    #region Poison damage
    /// <summary>
    /// Adds a damage over time effect to the enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageReduction"></param>
    /// <param name="timer"></param>
    public void PoisonDamage(int damage, float damageReduction, float timer, GameObject poisonOrbPrefab)
    {
        if(poisonTimer > 0)
        {
            if(damage > poisonDamageToTake) poisonDamageToTake = damage;
            if(damageReduction > poisonDamageReduction) poisonDamageReduction = damageReduction;
            poisonTimer += timer;
            this.poisonOrbPrefab = poisonOrbPrefab;
        } else
        {
            poisonTimer = timer;
            poisonDamageToTake = damage;
            poisonDamageReduction = damageReduction;
            this.poisonOrbPrefab = poisonOrbPrefab;
            GameManager.instance.soundEffectManager.PlaySound("Green");
        }
    }

    public bool isPoisoned()
    {
        return poisonTimer > 0;
    }

    #endregion

    #region Burn damage

    /// <summary>
    /// Adds a burning effect to the enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="timer"></param>
    /// <param name="range"></param>
    /// <param name="burnParticles"></param>
    public void BurnDamage(int damage, float timer, float range, GameObject burnParticles, GameObject floorParticles, bool mustBurn, int flames)
    {
        if(timer <= 0) return;
        if(damage <= 0) return;
        if(burning.timer > 0) 
        {
            return;
        }
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

        GameManager.instance.soundEffectManager.PlaySound("Orange");
        

        GameObject instantiatedParticles = GameObject.Instantiate(burnParticles, transform.position, transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;
        main.duration = timer;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();

        burning = (damage, timer, range, burnParticles, objs, floorParticles, mustBurn, instantiatedParticles, flames);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = gameObject.transform;

        for (int i = 0; i < flames; i++)
        {
            GameObject floorFlame = Instantiate(burning.floorParticles, GetPosition(), new Quaternion());
            floorFlame.GetComponent<FloorFlame>().dir = (int) ((i%2-0.5)*2);
            floorFlame.GetComponent<FloorFlame>().SetBurning(damage+6, timer+2, range, burnParticles, floorParticles, flames);
            }
    }

    public bool isBurning()
    {
        return burning.timer > 0;
    }

    public void StopBurning()
    {
        if(burning.enemyParticles != null)
            burning.enemyParticles.GetComponent<ParticleSystem>().Stop();
        burning = (0, 0, 0, null, null, null, false, null, 0);
    }

    #endregion

    #region Enemy Color
    
    /// <summary>
    /// Return what colorMat this enemy has and how much, then remove the colorMat form the enemy.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="AbsorbColor("></param>
    /// <returns></returns>
    public (GameColor color, int ammount) AbsorbColor()
    {
        GameColor ret = color;
        int ammount = colorAmmount;
        if (colorAmmount == 0) ret = null;
        RemoveColor();
        return (ret, ammount);
    }
    
    /// <summary>
    /// Removes the color from the enemy
    /// </summary>
    public void RemoveColor()
    {
        if (enemySleep) WakeEnemyAnimation();
        color = null;
        colorAmmount = 0;
        GetComponent<SpriteRenderer>().material = defaultColor;
        onColorChanged?.Invoke(null);
    }

    /// <summary>
    /// Check what colorMat this enemy has
    /// </summary>
    /// <returns></returns>
    public GameColor GetColor()
    {
        return color;
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        onColorChanged?.Invoke(color);
        if (enemySleep && lastSleep != Time.time) WakeEnemyAnimation();
        if (color != null)
        {
            GetComponent<SpriteRenderer>().material = color.colorMat;
            if(color.name.Equals("Rainbow"))
            {
                AchievementsManager.instance.ProgressAchievement("Painter");
                if (enemySleep && lastSleep == Time.time) colorComboTimer++;
            }
        }
        else
            GetComponent<SpriteRenderer>().material = defaultColor;
        
        isColoredThisFrame = true;
    }

    public void SetColor(GameColor color, int ammount)
    {
        SetColor(color);
        colorAmmount = ammount;
    }

    public int GetColorAmmount()
    {
        return colorAmmount;
    }

    #endregion

    #region Speed & Damage

    /// <summary>
    /// Change the enemys movement drag for a certain amount of time
    /// </summary>
    /// <param name="speedFactor">Is multiplied with the original drag</param>
    /// <param name="time"></param>
    public void ChangeDrag (float speedFactor, float time)
    {
        if (movementSpeedTimer > 0)
        {
            movementSpeedTimer = time;
            return;
        }
        animator.speed /= speedFactor;
        GetComponent<Rigidbody2D>().drag *= speedFactor;
        movementSpeedTimer = time;
    }

    /// <summary>
    /// Returns the enemys current speed
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return movementSpeed;
    }

    public void SetSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }

    public float GetDamageFactor()
    {
        float poisonFactor = 1f;
        if(isPoisoned())
            poisonFactor = (1f-poisonDamageReduction);
        
        float playerArmour = 0f;
        if(GetColor() != null && GetColorAmmount() > 0)
            playerArmour = playerStats.GetColorArmour(GetColor());
        return spawnPower * poisonFactor * (1f-playerArmour);
    }

    /// <summary>
    /// Gets the damage but scaled with the enemies various damage factors
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public int GetScaledDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage*GetDamageFactor());
    }
    
    /// <summary>
    /// Return the normal movement speed without any effects of this enemy
    /// </summary>
    /// <returns></returns>
    /*
    public float GetNormalMovementSpeed()
    {
        return normalMovementDrag;
    }*/
    
    /// <summary>
    /// Returs wether this enemy is knockbackimune
    /// </summary>
    /// <returns></returns>
    public bool IsKnockbackImune() { return knockbackImune; }

    #endregion

    #region Position

    public Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y) + myCollider.offset;
    }

    public void ChangeDirection()
    {
        GetComponent<Enemy>().SwitchDirection();
    }


    #endregion

    #region Sleep

    /// <summary>
    /// Sets the enemy to asleep for the specified time
    /// </summary>
    /// <param name="timer"></param>
    public void SleepEnemy(float timer, float sleepPower, GameObject particles)
    {
        if(sleepTimer <= 0)
            sleepPowerBonus = sleepPower;

        sleepTimer = timer;
        lastSleep = Time.time;
        enemySleep = true;
        if(particles){        
            GameObject instantiatedParticles = GameObject.Instantiate(particles, transform.position, transform.rotation);
            var main = instantiatedParticles.GetComponent<ParticleSystem>().main;
            main.duration = timer;
            instantiatedParticles.GetComponent<ParticleSystem>().Play();
            Destroy(instantiatedParticles, timer*1.2f);
            // Set enemy as parent of the particle system
            instantiatedParticles.transform.parent = transform;
            sleepParticles = instantiatedParticles;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>());
            }
        }
        animator.SetBool("sleep", true);
        onEnemySlept?.Invoke();
    }

    /// <summary>
    /// Starts the enemy wake animation
    /// </summary>
    private void WakeEnemyAnimation()
    {
        animator.SetBool("sleep", false); //TODO make animation event
        if(sleepParticles)
        {
            sleepParticles.GetComponent<ParticleSystem>().Stop();
            GameObject.Destroy(sleepParticles, 1f);
        }
    }

    /// <summary>
    /// Wakes the enemy
    /// </summary>
    public void WakeEnemy()
    {
        enemySleep = false;
        sleepTimer = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>(), false);
            }
        }
    }

    /// <summary>
    /// Retursn true if the enemy is asleep
    /// </summary>
    /// <returns></returns>
    public bool IsAsleep()
    {
        return enemySleep;
    }

    public float GetSleepPowerBonus()
    {
        if(!enemySleep) return 0;
        return sleepPowerBonus;
    }

    #endregion

    #region Coins

    public void dropCoins(int amount)
    {
        GameObject coin5 = Resources.Load<GameObject>("Coins/coin5");
        GameObject coin1 = Resources.Load<GameObject>("Coins/coin1");

        int coin5Amount = amount / 5;
        int coin1Amount = amount % 5;
        
        for (int i = 0; i < coin5Amount; i++)
        {
            Instantiate(coin5, transform.position, Quaternion.identity);
        }
        
        for (int i = 0; i < coin1Amount; i++)
        {
            Instantiate(coin1, transform.position, Quaternion.identity);
        }
    }

    #endregion
}

[System.Serializable]
public struct CoinRange
{
    [SerializeField] int min;
    [SerializeField] int max;

    public int GetReward()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
            return (int)(UnityEngine.Random.Range(min, max+1)*player.GetComponent<ItemInventory>().coinBoost);
        else return UnityEngine.Random.Range(min, max+1);
    }
}
