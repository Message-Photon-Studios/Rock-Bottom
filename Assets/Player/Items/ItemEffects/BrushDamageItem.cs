using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds damage to the players pick up color ability.
/// </summary>
[System.Serializable]
public class BrushDamageItem : ItemEffect
{
    [SerializeField] int brushDamage;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().defaultAttackDamage += brushDamage;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().defaultAttackDamage -= brushDamage;
    }
}
