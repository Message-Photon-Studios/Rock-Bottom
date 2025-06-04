using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowMaxedBuff : ItemEffect
{
    [Header("Rainbow Combo Damage")]
    [SerializeField] int damage;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().rainbowedDamage += damage;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().rainbowedDamage -= damage;
    }
}
