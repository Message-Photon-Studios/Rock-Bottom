using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The color effects handle the effect that the color spell will have when impacting the enemy. 
/// </summary>
public abstract class ColorEffect : ScriptableObject
{
    [SerializeField] protected GameObject particles;
    [SerializeField] protected int damage;

    /// <summary>
    /// Apply this color effect to the enemy
    /// </summary>
    /// <param name="enemyObj"></param>
    /// <param name="playerObj"></param>
    /// <param name="power"></param>
    public abstract void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer);

    /// <summary>
    /// The effect over power curve
    /// </summary>
    /// <param name="x"> The power of the spell</param>
    /// <returns></returns>
    protected float EffectFunction (float x)
    {
        return (1/(1+Mathf.Exp(-(Mathf.Pow(x*100, 1/1.4f)*5-165)/20))*110+80)/100;
    }
}
