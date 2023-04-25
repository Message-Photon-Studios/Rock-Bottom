using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemEffect
{  
    public abstract void ActivateEffect();

    protected GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
