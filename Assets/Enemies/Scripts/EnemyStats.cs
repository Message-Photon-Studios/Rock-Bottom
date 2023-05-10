using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Important stats for an enemy.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class EnemyStats : MonoBehaviour
{
    [SerializeField] float health; //The health of the enemy
    [SerializeField] GameColor color; //The colorMat of the enemy
    [SerializeField] int colorAmmount; //The ammount of colorMat you will get when absorbing the colorMat from the enemy
    [SerializeField] float movementSpeed; //The current movement speed of the enemy
    [SerializeField] CoinRange coinsDropped; //Keeps track of how much coins this enemy drops upon death

    private Collider2D myCollider;
    [SerializeField] private Material defaultColor; //The material that is used when there is no GameColor attached

    [SerializeField] private float sleepForcedown; //The force downwards that will be applied to a sleeping enemy

    /// <summary>
    /// The direction that the enemy is looking
    /// </summary>
    public float lookDir = -1;

    private float normalMovementSpeed; //The normal movement speed of the enemy
    private float movementSpeedTimer; 

    bool enemySleep = false; //If the enemy sleep is true the enemy will be inactive
    private float sleepTimer = 0; 
    private float sleepDamageBonus = 1.2f; //The extra damage dealt to a slept enemy
    GameObject sleepParticles;
    private float comboTime = 1; //The timelimit for the next move of a combo
    private float comboTimer = 0;
    private GameColor comboColor; //The colorMat that currently affects the enemy in a combo
    [HideInInspector] public int currentCombo = 0; //At what stage this combo is at

    private float secTimer = 0f;

    private Animator animator;

    [HideInInspector] public EnemySounds enemySounds;

    private List<(float damage, float timer)> poisonEffects = new List<(float damage, float time)>(); //Damage dealt over time

    private (float damage, float timer, float range, GameObject particles, GameObject[] burnable) burning;
    /// <summary>
    /// This event fires when the enemys health is changed. The float is the damage received.
    /// </summary>
    public UnityAction<float> onHealthChanged;
    public UnityAction<float, Vector2> onDamageTaken;

    /// <summary>
    /// The enemy died
    /// </summary>
    public UnityAction onEnemyDeath;

    [CanBeNull] private Coroutine currentCoroutine;

    #region Setup and Timers
    void Awake()
    {
        color = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>().GetRandomEnemyColor();
        normalMovementSpeed = movementSpeed;
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().material = color.colorMat;
    }

    void Start()
    {
        onDamageTaken += DmgNumber.create;
        enemySounds = GetComponent<EnemySounds>();
    }

    void OnValidate()
    {
        myCollider = GetComponent<Collider2D>();
        GetComponent<SpriteRenderer>().material = color.colorMat;
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
        movementSpeed = normalMovementSpeed;
        poisonEffects = new List<(float damage, float timer)>();
        burning = (0, 0, 0, null, null);
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_takingDmg", 0);
    }

    void Update()
    {
        if(secTimer > 1f)
        {
            if(comboTimer > 0)
            {
                comboTimer --;
                if(comboTimer <= 0) 
                {
                    comboTimer = 0;
                    comboColor = null;
                    currentCombo = 0;
                }
            }

            if(movementSpeedTimer > 0)
            {
                movementSpeedTimer --;
                if(movementSpeedTimer <= 0)
                {
                    movementSpeedTimer = 0;
                    movementSpeed = normalMovementSpeed;
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

            if(poisonEffects.Count > 0)
            {
                for (int i = 0; i < poisonEffects.Count; i++)
                {
                    float damage = poisonEffects[i].damage;
                    if (damage >= health)
                    {
                        damage = 0;
                    }
                    
                    DamageEnemy(damage);
                    poisonEffects[i] = (damage, poisonEffects[i].timer -1);
                    if(poisonEffects[i].timer <= 0)
                    {
                        poisonEffects.RemoveAt(i);
                        i--;
                    }
                }
            }

            if(burning.timer > 0)
            {
                if(color?.name != "Orange" || color == null) DamageEnemy(burning.damage);
                else DamageEnemy(0);
                float timer = burning.timer;
                timer --;
                burning.timer = timer;

                //Debug.Log("burning: d " + burning.damage + " : t " + burning.timer);

                if(timer <= 0)
                {
                    burning = (0, 0, 0, null, null);
                    return;
                }
                

                foreach(GameObject obj in burning.burnable)
                {
                    if(obj == null) return;
                    float dist = Vector2.Distance(transform.position, obj.transform.position);
                    if(dist < burning.range)
                    {
                        obj.GetComponent<EnemyStats>()?.BurnDamage(burning.damage, burning.timer, burning.range, burning.particles);
                    }
                }
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

    #region Damage and Kill
    /// <summary>
    /// Damage the enemy with the specified damage
    /// </summary>
    /// <param name="damage"></param>
    public void DamageEnemy(float damage)
    {
        if(enemySleep)
        {
            damage *= sleepDamageBonus;
            WakeEnemyAnimation();
        }

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
    /// Adds a damage over time effect to the enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="timer"></param>
    public void PoisonDamage(float damage, float timer)
    {
        poisonEffects.Add((damage, timer));
    }

    public bool isPoisoned()
    {
        return poisonEffects.Count > 0;
    }

    /// <summary>
    /// Adds a burning effect to the enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="timer"></param>
    /// <param name="range"></param>
    /// <param name="burnParticles"></param>
    public void BurnDamage(float damage, float timer, float range, GameObject burnParticles)
    {
        if(timer <= 0) return;
        if(burning.timer > 0) return;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        burning = (damage, timer, range, burnParticles, objs);

        GameObject instantiatedParticles = GameObject.Instantiate(burnParticles, transform.position, transform.rotation);
        var main = instantiatedParticles.GetComponent<ParticleSystem>().main;
        main.duration = timer;
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, timer*1.2f);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = gameObject.transform;
    }

    public bool isBurning()
    {
        return burning.timer > 0;
    }

    /// <summary>
    /// Kill then enemy
    /// </summary>
    public void KillEnemy()
    {
        enemySounds?.PlayDeath();
        //TODO
        if (animator.GetBool("dead")) return;
        Debug.Log(gameObject.name + " died");
        animator.SetBool("dead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5);
        SleepEnemy(10, 1, null);
        GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>().AddCoins(coinsDropped.GetReward());
        onEnemyDeath?.Invoke();
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
        color = null;
        colorAmmount = 0;
        GetComponent<SpriteRenderer>().material = defaultColor;
        return (ret, ammount);
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
        if(color != null)
            GetComponent<SpriteRenderer>().material = color.colorMat;
        else
            GetComponent<SpriteRenderer>().material = defaultColor;
    }

    #endregion

    #region Combo Color

    /// <summary>
    /// Sets the combo colorMat for the player
    /// </summary>
    /// <param name="comboColor"></param>
    public void SetComboColor(GameColor comboColor)
    {
        this.comboColor = comboColor;
        comboTimer = comboTime;
    }

    /// <summary>
    /// Get the current combo colorMat of the player
    /// </summary>
    /// <returns></returns>
    public GameColor GetComboColor()
    {
        return comboColor;
    }

    #endregion

    #region Movement Speed

    /// <summary>
    /// Change the enemys movement speed for a certain amount of time
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="time"></param>
    public void ChangeSpeed (float speed, float time)
    {
        movementSpeed = speed;
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
    
    /// <summary>
    /// Return the normal movement speed without any effects of this enemy
    /// </summary>
    /// <returns></returns>
    public float GetNormalMovementSpeed()
    {
        return normalMovementSpeed;
    }

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
        if(sleepTimer > 0)
        {
            if(sleepDamageBonus < sleepPower) sleepDamageBonus = sleepPower;
        } else sleepDamageBonus = sleepPower;

        sleepTimer = timer;
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

        animator.SetBool("sleep", true);
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
    }

    /// <summary>
    /// Retursn true if the enemy is asleep
    /// </summary>
    /// <returns></returns>
    public bool IsAsleep()
    {
        return enemySleep;
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
        return UnityEngine.Random.Range(min, max+1);
    }
}
