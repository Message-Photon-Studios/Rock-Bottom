using System.Collections.Generic;
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

    private Collider2D myCollider;  
    [SerializeField] private Material defaultColor; //The material that is used when there is no GameColor attached

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

    private Animator animator;

    private List<(float damage, float timer)> poisonEffects = new List<(float damage, float time)>(); //Damage dealt over time
    
    private (float damage, float timer, float range, GameObject particles, GameObject[] burnable) burning;
    /// <summary>
    /// This event fires when the enemys health is changed. The float is the new health.
    /// </summary>
    public UnityAction<float> onHealthChanged;

    /// <summary>
    /// The enemy died
    /// </summary>
    public UnityAction onEnemyDeath;

    #region Setup and Timers
    void OnEnable()
    {
        color = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>().GetRandomEnemyColor();
        normalMovementSpeed = movementSpeed;
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().material = color.colorMat;
        onHealthChanged?.Invoke(health);
    }

    void OnValidate()
    {
        myCollider = GetComponent<Collider2D>();
        GetComponent<SpriteRenderer>().material = color.colorMat;
    }

    void Update()
    {
        if(comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if(comboTimer <= 0) 
            {
                comboTimer = 0;
                comboColor = null;
                currentCombo = 0;
            }
        }

        if(movementSpeedTimer > 0)
        {
            movementSpeedTimer -= Time.deltaTime;
            if(movementSpeedTimer <= 0)
            {
                movementSpeedTimer = 0;
                movementSpeed = normalMovementSpeed;
            }
        }

        if(sleepTimer > 0)
        {
            sleepTimer -= Time.deltaTime;
            if(sleepTimer <= 0)
            {
                sleepTimer = 0;
                WakeEnemy();
            }
        }

        if(poisonEffects.Count > 0)
        {
            for (int i = 0; i < poisonEffects.Count; i++)
            {
                float damage = poisonEffects[i].damage * Time.deltaTime;
                if (damage >= health)
                {
                    poisonEffects.RemoveAt(i);
                    i--;
                    continue;
                }
                
                DamageEnemy(damage);
                poisonEffects[i] = (poisonEffects[i].damage, poisonEffects[i].timer - Time.deltaTime);
                if(poisonEffects[i].timer <= 0)
                {
                    poisonEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        if(burning.timer > 0)
        {
            DamageEnemy(burning.damage * Time.deltaTime);
            float timer = burning.timer;
            timer -= Time.deltaTime;
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
            WakeEnemy();
        }

        health -= damage;

        onHealthChanged?.Invoke(health);
        if(health <= 0) KillEnemy();
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

    /// <summary>
    /// Kill then enemy
    /// </summary>
    public void KillEnemy()
    {
        //TODO
        Debug.Log(gameObject.name + " died");
        animator.SetBool("dead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        SleepEnemy(10, 1, null);
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
    }

    /// <summary>
    /// Wakes the enemy
    /// </summary>
    public void WakeEnemy()
    {
        enemySleep = false;
        sleepTimer = 0;
        if(sleepParticles)
        {
            sleepParticles.GetComponent<ParticleSystem>().Stop();
            GameObject.Destroy(sleepParticles, 1f);
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

    #endregion
}
