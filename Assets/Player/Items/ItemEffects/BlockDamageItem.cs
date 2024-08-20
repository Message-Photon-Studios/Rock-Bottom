using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamageItem : ItemEffect
{
    [SerializeField] int addBlockChance;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceToBlock += addBlockChance;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceToBlock -= addBlockChance;
    }

    public override bool CanBeSpawned()
    {
        return GetPlayer().GetComponent<PlayerStats>().chanceToBlock < 50;
    }
}
