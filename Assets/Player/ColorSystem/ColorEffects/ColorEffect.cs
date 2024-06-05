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
    public abstract void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power);

    public abstract string UpdateDesc(float power);
}
