using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A standard item effect
/// </summary>
[System.Serializable]
public abstract class ItemEffect
{  
    public abstract void ActivateEffect();

    public abstract void DisableEffect();

    protected GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Retuns true if the item can be spawned
    /// </summary>
    /// <returns></returns>
    public virtual bool CanBeSpawned()
    {
        return true;
    }
}
