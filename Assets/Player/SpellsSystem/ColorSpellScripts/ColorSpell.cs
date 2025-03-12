using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Handles the impact of color spells
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColorSpell : MonoBehaviour
{
    [SerializeField] public LocalizedString bottleName;
    [SerializeField] public LocalizedString description;

    public int spellCost;
    /// <summary>
    /// Scales the power for this specific color spell
    /// </summary>
    [SerializeField] public float powerScale = 1;
    [SerializeField] public float coolDown = 1;
    [SerializeField] public bool castWhenDamaged;
    [SerializeField] public bool castOnSpellImpact;
    [SerializeField] public bool castOnDash;
    [SerializeField] public bool castOnDoubleJump;
    [SerializeField] public bool staggeredSpell;

    [SerializeField] public int storedSpells = 1;

    [SerializeField] public int maxSpawn = 0;

    [SerializeField] public string spawnKey;

    [SerializeField] bool ignoreLookDir;

    /// <summary>
    /// The projectile will be destroyed on impact with any object
    /// </summary>
    [SerializeField] protected bool destroyOnAllImpact;

    /// <summary>
    /// The projectile will be destroyed on impact with the enemy
    /// </summary>
    [SerializeField] protected bool destroyOnEnemyHit;

    [SerializeField] protected bool destroyOnCollission;

    /// <summary>
    /// If true this spell will detect enemies as a hit
    /// </summary>
    [SerializeField] protected bool impactOnEnemies = true;

    /// <summary>
    /// If true this spell will detect non-enemies as a hit
    /// </summary>
    [SerializeField] protected bool impactOnNonEnemies = true;
    /// <summary>
    /// If true the spell will only trigger once.
    /// </summary>
    [SerializeField] protected bool triggerOnlyOnce;

    /// <summary>
    /// If true the spell will only trigger once per enemy. It can still hit multiple different enemies.
    /// </summary>
    [SerializeField] protected bool triggerOnlyOncePerEnemy;

    /// <summary>
    /// How often enemy already triggered will be reset
    /// </summary>
    [SerializeField] protected float attackAgainTimer = -1;
    protected float resetEnemyTime;

    /// <summary>
    /// If true the spell checks if it has LOS to the player on spawn and destroys itself if not
    /// </summary>
    [SerializeField] protected bool requirePlayerLOSonSpawn;
    protected bool hasTriggered = false;
    /// <summary>
    /// The maximum lifetime of the projectile
    /// </summary>
    [SerializeField] float lifeTime;

    /// <summary>
    /// The animation event that this spell uses on the ground
    /// </summary>
    [SerializeField] string animationTrigger;



    /// <summary>
    /// The things that will trigger on impact
    /// </summary>
    [SerializeField] SpellImpact[] onImpact;

    /// <summary>
    /// The sprite used for this spell
    /// </summary>
    [SerializeField] BottleSprite spellSprite;
    protected GameColor gameColor;
    protected float power;
    protected GameObject player;
    
    protected int extraDamage;
    public int lookDir {get; protected set;}

    private HashSet<Collider2D> objectsAlreadyHit = new HashSet<Collider2D>();

    /// <summary>
    /// Needs to be called after the spell is instantiated
    /// </summary>
    /// <param name="gameColor">The color effect of the spell</param>
    /// <param name="power">The total power of the spell</param>
    /// <param name="player">The player object</param>
    /// <param name="lookDir">The direction the spell should face horizontally</param>
    public void Initi(GameColor gameColor, float power, GameObject player, int lookDir, int extraDamage)
    {
        this.gameColor = gameColor;
        this.power = power+powerScale;
        this.player = player;
        this.lookDir = lookDir;
        this.extraDamage = extraDamage;
        resetEnemyTime = attackAgainTimer;

        foreach(Light2D light in GetComponentsInChildren<Light2D>())
        {
            light.color = gameColor.lightTintColor;
        }

        if (this.power <= 0.1) this.power = 0.1f;

        if (!ignoreLookDir)
        {
            transform.localScale = new Vector3(lookDir,1,1);
            /*foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.offset *= new Vector2(lookDir, 1);
            }*/
        }
        
        var spriteRenderer = GetComponent<SpriteRenderer>();
        /*if (spriteRenderer != null && !ignoreLookDir)
        {
            spriteRenderer.flipX = lookDir == -1;
        }*/
        spriteRenderer.material = gameColor?.colorMat;

        LineRenderer line = GetComponent<LineRenderer>();
        if (line) line.material = gameColor?.colorMat;

        foreach(var child in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            /*
            if (child != null && !ignoreLookDir)
            {
                child.flipX = lookDir == -1;
            }*/
            child.material = gameColor?.colorMat;
        }
        
        foreach (var ballTray in GetComponentsInChildren<ParticleSystem>())
        {
            var main = ballTray.main;
            main.startColor = gameColor.colorMat.color;
            ballTray.Play();
            /*
            if (!ignoreLookDir)
                ballTray.transform.localPosition = new Vector2(ballTray.transform.localPosition.x * lookDir, ballTray.transform.localPosition.y);
                */
        }

        foreach (SpellMover mover in gameObject.GetComponents<SpellMover>())
        {
            mover.Init(lookDir);
        }

        if (this != null)
        {
            foreach (SpellImpact impact in onImpact)
            {
                impact.Init(this);
            }
        }
        

        objectsAlreadyHit = new HashSet<Collider2D>();

        if(requirePlayerLOSonSpawn)
        {
            RaycastHit2D playerLOS = Physics2D.Raycast(transform.position, player.transform.position-transform.position, Vector2.Distance(transform.position, player.transform.position), GameManager.instance.maskLibrary.onlyGround);
            if(playerLOS.collider != null) 
            {
                if(impactOnNonEnemies) Impact(playerLOS.collider, transform.position);
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetNewDestroy(float time)
    {
        Destroy(gameObject, time);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(triggerOnlyOnce && hasTriggered) return;
        if(other.CompareTag("Item") || other.CompareTag("Player")) return;
        if(!impactOnEnemies && other.CompareTag("Enemy")) return;
        if(!impactOnNonEnemies && !other.CompareTag("Enemy")) return;
        if(objectsAlreadyHit.Contains(other)) return;
        hasTriggered = true;
        Impact(other, GetComponent<Collider2D>().ClosestPoint(other.transform.position));
        objectsAlreadyHit.Add(other);
        if(attackAgainTimer < 0.5f) attackAgainTimer += 0.5f;

        if(destroyOnAllImpact)
        {
            Destroy(gameObject);
            return;
        }

        if(destroyOnEnemyHit && other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddObjectAlreadyHit(Collider2D collider) 
    {
        if (objectsAlreadyHit.Contains(collider)) return;
        objectsAlreadyHit.Add(collider);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (destroyOnCollission)
        {
            Impact(other.collider, GetComponent<Collider2D>().ClosestPoint(other.transform.position));
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(triggerOnlyOnce) return;
        if(triggerOnlyOncePerEnemy) return;
        if(!objectsAlreadyHit.Contains(other)) return;

        objectsAlreadyHit.Remove(other);
    }

    private void Update() {
        if(!triggerOnlyOnce && resetEnemyTime > 0f && objectsAlreadyHit.Count > 0 && !triggerOnlyOncePerEnemy)
        {
            if(attackAgainTimer > 0) attackAgainTimer -= Time.deltaTime;
            else
            {
                try{
                    foreach (Collider2D obj in objectsAlreadyHit)
                    {
                        if(obj != null)
                            Impact(obj, GetComponent<Collider2D>().ClosestPoint(obj.transform.position));
                    }
                } catch (InvalidOperationException e)
                {
                    Debug.LogWarning(e);
                }
                attackAgainTimer = resetEnemyTime;
            }
        }
    }

    /// <summary>
    /// This is called when the spell should do its effect
    /// </summary>
    /// <param name="other"></param>
    void Impact(Collider2D other, Vector2 impactPoint)
    {
        foreach (SpellImpact impact in onImpact)
        {
            impact.Impact(other, impactPoint);
        }
    }

    /// <summary>
    /// Returns the name of the animation trigger that should be used by this spell.
    /// </summary>
    /// <returns></returns>
    public string GetAnimationTrigger()
    {
        return animationTrigger;
    }

    /// <summary>
    /// Returns the color of this color spell
    /// </summary>
    /// <returns></returns>
    public GameColor GetColor()
    {
        return gameColor;
    }

    /// <summary>
    /// Returns the total power of this color spell
    /// </summary>
    /// <returns></returns>
    public float GetPower()
    {
        return power;
    }


    public GameObject GetPlayerObj()
    {
        return player;
    }

    /// <summary>
    /// Returns the extra damage the spell should do
    /// </summary>
    /// <returns></returns>
    public int GetExtraDamage()
    {
        return extraDamage;
    }

    /// <summary>
    /// Returns the sprite of this color spell
    /// </summary>
    /// <returns></returns>
    public BottleSprite GetBottleSprite()
    {
        return spellSprite;
    }

    /// <summary>
    /// Gives name of ColorSpell depending on which language is selected.
    /// </summary>
    /// <returns>Localized ColorSpell Name.</returns>
    public string GetName() {
        return bottleName.GetLocalizedString();
    }

    /// <summary>
    /// Gives description of ColorSpell depending on which language is selected.
    /// </summary>
    /// <returns>Localized ColorSpell description.</returns>
    public string GetDesc() {
        return description.GetLocalizedString();
    }

    public void SetDir(int lookDir)
    {
        /*
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.offset *= new Vector2(lookDir, 1);
        }
        */

        //var spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(lookDir,1,1);
        //spriteRenderer.material = gameColor?.colorMat;

    /*
        foreach (var child in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child != null)
            {
                child.flipX = lookDir == -1;
            }
            child.material = gameColor?.colorMat;
        }*/
    }

    public void TriggerQueue()
    {
        player.GetComponent<ColorInventory>().QuedSpells(spawnKey);
    }

    public void DestroySpell()
    {
        Destroy(gameObject);
    }
}
/// <summary>
/// This struct keeps track of all sprites for a bottle
/// </summary>
[System.Serializable]
public struct BottleSprite
{
    public Sprite bigSprite;
    public Sprite bigSpriteMask;
    public Sprite bigSpriteCapMask;
    public Sprite mediumSprite;
    public Sprite mediumSpriteMask;
    public Sprite mediumSpriteCapMask;
    public Sprite smallSprite;
    public Sprite smallSpriteMask;
    public Sprite smallSpriteCapMask;
}