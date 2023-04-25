using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    [SerializeField] Sprite sprite;   
    public abstract void ActivateEffect();

    public Sprite GetSprite()
    {
        return sprite;
    }

    protected GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
