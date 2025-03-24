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
    [SerializeField] int addMaxPermaTHP;
    public override void ActivateEffect()
    {
        PlayerStats playerStats = GetPlayer().GetComponent<PlayerStats>();
        playerStats.AddMaxShield(addMaxTHP);
        playerStats.AddMaxPermanentShield(addMaxPermaTHP);
    }

    public override void DisableEffect()
    {
        PlayerStats playerStats = GetPlayer().GetComponent<PlayerStats>();
        playerStats.AddMaxShield(-addMaxTHP);
        playerStats.AddMaxPermanentShield(-addMaxPermaTHP);
    }
}
