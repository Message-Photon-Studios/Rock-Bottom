using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the impact of color spells
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColorSpell : MonoBehaviour
{
    [TextArea(5,20)] public string description;

    public int spellCost;
    /// <summary>
    /// Scales the power for this specific color spell
    /// </summary>
    [SerializeField] public float powerScale = 1;

    /// <summary>
    /// The projectile will be destroyed on impact with any object
    /// </summary>
    [SerializeField] protected bool destroyOnAllImpact;

    /// <summary>
    /// The projectile will be destroyed on impact with the enemy
    /// </summary>
    [SerializeField] protected bool destroyOnEnemyHit;

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

    public int lookDir {get; protected set;}

    private HashSet<GameObject> objectsAlreadyHit = new HashSet<GameObject>();

    /// <summary>
    /// Needs to be called after the spell is instantiated
    /// </summary>
    /// <param name="gameColor">The color effect of the spell</param>
    /// <param name="power">The total power of the spell</param>
    /// <param name="player">The player object</param>
    /// <param name="lookDir">The direction the spell should face horizontally</param>
    public void Initi(GameColor gameColor, float power, GameObject player, int lookDir)
    {
        this.gameColor = gameColor;
        this.power = power+powerScale;
        this.player = player;
        this.lookDir = lookDir;

        foreach(Collider2D col in GetComponents<Collider2D>())
        {
            col.offset *= new Vector2(lookDir, 1);
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = lookDir == -1;
        }
        spriteRenderer.material = gameColor?.colorMat;

        foreach(var child in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child != null)
            {
                child.flipX = lookDir == -1;
            }
            child.material = gameColor?.colorMat;
        }
        
        var ballTray = GetComponentInChildren<ParticleSystem>();
        if (ballTray != null)
        {
            var main = ballTray.main;
            main.startColor = gameColor.colorMat.color;
            ballTray.Play();
        }
        // Initialize the spell movers
        

        foreach (SpellMover mover in gameObject.GetComponents<SpellMover>())
        {
            mover.Init(lookDir);
        }

        foreach(SpellImpact impact in onImpact)
        {
            impact.Init(this);
        }

        objectsAlreadyHit = new HashSet<GameObject>();

        if(requirePlayerLOSonSpawn)
        {
            RaycastHit2D playerLOS = Physics2D.Raycast(transform.position, player.transform.position-transform.position, Vector2.Distance(transform.position, player.transform.position), GameManager.instance.maskLibrary.onlyOnlyGround);
            if(playerLOS.collider != null) 
            {
                if(impactOnNonEnemies) Impact(playerLOS.collider);
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(triggerOnlyOnce && hasTriggered) return;
        if(other.CompareTag("Item") || other.CompareTag("Player")) return;
        if(!impactOnEnemies && other.CompareTag("Enemy")) return;
        if(!impactOnNonEnemies && !other.CompareTag("Enemy")) return;
        if(objectsAlreadyHit.Contains(other.gameObject)) return;
        hasTriggered = true;
        Impact(other);
        objectsAlreadyHit.Add(other.gameObject);

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

    /// <summary>
    /// This is called when the spell should do its effect
    /// </summary>
    /// <param name="other"></param>
    void Impact(Collider2D other)
    {
        foreach (SpellImpact impact in onImpact)
        {
            impact.Impact(other);
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
    /// Returns the sprite of this color spell
    /// </summary>
    /// <returns></returns>
    public BottleSprite GetBottleSprite()
    {
        return spellSprite;
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