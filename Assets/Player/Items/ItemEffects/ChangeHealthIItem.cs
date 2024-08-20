using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the players health and max health
/// </summary>
public class ChangeHealthIItem : ItemEffect
{
    [SerializeField] int addMaxHealth;
    [SerializeField] int healAmmount;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().AddMaxHealth(addMaxHealth);
        GetPlayer().GetComponent<PlayerStats>().HealPlayer(healAmmount);
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().AddMaxHealth(-addMaxHealth);
    }
}
