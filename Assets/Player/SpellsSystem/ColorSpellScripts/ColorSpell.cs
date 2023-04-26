using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the impact of color spells
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColorSpell : MonoBehaviour
{
    /// <summary>
    /// Scales the power for this specific color spell
    /// </summary>
    [SerializeField] protected float powerScale = 1;

    /// <summary>
    /// The projectile will be destroyed on impact with any object
    /// </summary>
    [SerializeField] protected bool destroyOnImpact;

    /// <summary>
    /// The projectile will be destroyed on impact with the enemy
    /// </summary>
    [SerializeField] protected bool destroyOnHit;

    /// <summary>
    /// The maximum lifetime of the projectile
    /// </summary>
    [SerializeField] float lifeTime;

    /// <summary>
    /// The animation event that this spell uses
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

    private HashSet<string> hitEnemies = new HashSet<string>();

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
        spriteRenderer.material = gameColor.colorMat;
        
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

        hitEnemies = new HashSet<string>();
    }

    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(destroyOnImpact && !hitEnemies.Contains(other.gameObject.name)) 
        {
            Impact(other);
            Destroy(gameObject);
            hitEnemies.Add(other.gameObject.name);
            return;
        }

        if(other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject.name))
        {
            Impact(other);
            hitEnemies.Add(other.gameObject.name);
            if(destroyOnHit) Destroy(gameObject);
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
}