using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
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

    [SerializeField] private float sleepForcedown; //The force downwards that will be applied to a sleeping enemy

    [SerializeField] private bool setColorByHand;

    /// <summary>
    /// The direction that the enemy is looking
    /// </summary>
    public float lookDir = -1;

    private float normalMovementSpeed; //The normal movement speed of the enemy
    private float movementSpeedTimer;

    private int colorComboDamage = 40; // The damage that the enemy will take when becoming rainbow color
    private float colorComboTimer = 2f; //The timer before the enemy explode

    bool enemySleep = false; //If the enemy sleep is true the enemy will be inactive
    private float sleepTimer = 0; 
    private float sleepPowerBonus = 1.2f; //The extra damage dealt to a slept enemy
    GameObject sleepParticles;
    [HideInInspector] public int currentCombo = 0; //At what stage this combo is at

    private float secTimer = 0f; //Makes sure that all timers are updated only each second. 

    private Animator animator;

    [HideInInspector] public EnemySounds enemySounds;

    private List<(int damage, float timer)> poisonEffects = new List<(int damage, float time)>(); //Damage dealt over time

    private (int damage, float timer, float range, GameObject particles, GameObject[] burnable) burning;
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
        if(!setColorByHand)
            color = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>().GetRandomEnemyColor();
        normalMovementSpeed = movementSpeed;
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        if(color != null)
            GetComponent<SpriteRenderer>().material = color.colorMat;
        else
            GetComponent<SpriteRenderer>().material = defaultColor;
    }

    void Start()
    {
        onDamageTaken += DmgNumber.create;
        onEnemyDeath += () => dropCoins(coinsDropped.GetReward());
        enemySounds = GetComponent<EnemySounds>();
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
        movementSpeed = normalMovementSpeed;
        poisonEffects = new List<(int damage, float timer)>();
        burning = (0, 0, 0, null, null);
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_takingDmg", 0);
    }

    void Update()
    {
        if(secTimer > 1f)
        {
            if(color != null && color.name == "Rainbow")
            {
                colorComboTimer--;

                if (colorComboTimer <= 0)
                {
                    DamageEnemy(colorComboDamage);
                    AbsorbColor();
                    colorComboTimer = 2f;

                    GameObject instantiatedParticles = GameObject.Instantiate(comboParticles, transform.position, transform.rotation);
                    instantiatedParticles.GetComponent<ParticleSystem>().Play();
                    Destroy(instantiatedParticles, 1f);
                    // Set enemy as parent of the particle system
                    instantiatedParticles.transform.parent = transform;
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
                    int damage = poisonEffects[i].damage;
                    if (damage >= health)
                    {
                        if(damage > 1)
                            damage = health -1;
                        else damage = 0;
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
                DamageEnemy(burning.damage);
                //if(color?.name != "Orange" || color == null) DamageEnemy(burning.damage);
                //else DamageEnemy(0);
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
    public void DamageEnemy(int damage)
    {
        if(enemySleep)
        {
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
    public void PoisonDamage(int damage, float timer)
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
    public void BurnDamage(int damage, float timer, float range, GameObject burnParticles)
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
        onEnemyDeath?.Invoke();
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
        color = null;
        colorAmmount = 0;
        GetComponent<SpriteRenderer>().material = defaultColor;
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
            if(sleepPowerBonus < sleepPower) sleepPowerBonus = sleepPower;
        } else sleepPowerBonus = sleepPower;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
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
        return UnityEngine.Random.Range(min, max+1);
    }
}
