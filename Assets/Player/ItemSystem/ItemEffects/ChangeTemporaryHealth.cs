using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the players health and max health
/// </summary>
public class ChangeTemporaryHealth : ItemEffect
{
    [Header("Add Max Temp Health")]
    [SerializeField] int addMaxTHP;
    [SerializeField] int addPermaMaxTHP;
    public override void ActivateEffect()
    {
        PlayerStats playerStats = GetPlayer().GetComponent<PlayerStats>();
        playerStats.AddMaxShield(addMaxTHP);
    }

    public override void DisableEffect()
    {
    }
}
