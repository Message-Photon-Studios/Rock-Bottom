using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds damage to the players pick up color ability.
/// </summary>
[System.Serializable]
public class BrushDamageItem : ItemEffect
{
    [Header("Grey Extra Damage")]
    [SerializeField] int greyExtraDamage;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().greyExtraDamage += greyExtraDamage;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().greyExtraDamage -= greyExtraDamage;
    }
}
