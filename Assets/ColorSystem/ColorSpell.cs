using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the impact of color spells
/// </summary>
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
    protected ColorEffect colorEffect;
    protected float power;
    protected GameObject player;

    public void Initi(ColorEffect colorEffect, float power, GameObject player)
    {
        this.colorEffect = colorEffect;
        this.power = power;
        this.player = player;
    }

    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(destroyOnImpact) 
        {
            Impact(other);
            Destroy(gameObject);
        }

        if(other.collider.CompareTag("Enemy"))
        {
            Impact(other);
            if(destroyOnHit) Destroy(gameObject);
        }
    }

    protected abstract void Impact(Collision2D other);
}
