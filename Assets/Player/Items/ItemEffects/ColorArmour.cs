using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorArmour : ItemEffect
{
    [SerializeField] GameColor color;
    [SerializeField] float armour;

    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().AddColorArmour(color,armour);
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().AddColorArmour(color,-armour);
    }

    public override bool CanBeSpawned()
    {
        return GetPlayer().GetComponent<PlayerStats>().GetColorArmour(color) <= 50;
    }
}
