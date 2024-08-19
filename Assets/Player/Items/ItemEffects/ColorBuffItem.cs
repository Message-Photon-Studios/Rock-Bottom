using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Power up the players colors
/// </summary>
[System.Serializable]
public class ColorBuffItem : ItemEffect
{
    [SerializeField] GameColor color;
    [SerializeField] float power;
    public override void ActivateEffect()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        if(inv.colorBuffs.ContainsKey(color)) inv.colorBuffs[color] += power;
        else inv.colorBuffs.Add(color, power);
    }

    public override void DisableEffect()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        if(inv.colorBuffs.ContainsKey(color)) inv.colorBuffs[color] -= power;
    }
}
