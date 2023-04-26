using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHealthIItem : ItemEffect
{
    [SerializeField] float addMaxHealth;
    [SerializeField] float healAmmount;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().AddMaxHealth(addMaxHealth);
        GetPlayer().GetComponent<PlayerStats>().HealPlayer(healAmmount);
    }
}