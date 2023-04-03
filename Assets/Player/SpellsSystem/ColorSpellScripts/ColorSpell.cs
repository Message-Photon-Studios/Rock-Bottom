using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the impact of color spells
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class ColorSpell : MonoBehaviour
{
    /// <summary>
    /// Scales the power for this specific color spell
    /// </summary>
    [SerializeField] protected float powerScale;

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

    protected GameColor gameColor;
    protected float power;
    protected GameObject player;

    public int lookDir {get; protected set;}

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
        this.power = power;
        this.player = player;
        this.lookDir = lookDir; //TODO: Flip sprites

        foreach (SpellMover mover in gameObject.GetComponents<SpellMover>())
        {
            mover.Init(lookDir);
        }
    }

    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(destroyOnImpact) 
        {
            Impact(other);
            Destroy(gameObject);
            return;
        }

        if(other.CompareTag("Enemy"))
        {
            Impact(other);
            if(destroyOnHit) Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// This is called when the spell should do its effect
    /// </summary>
    /// <param name="other"></param>
    protected abstract void Impact(Collider2D other);

    /// <summary>
    /// Returns the name of the animation trigger that should be used by this spell.
    /// </summary>
    /// <returns></returns>
    public string GetAnimationTrigger()
    {
        return animationTrigger;
    }
}
