using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDontMixItem : ItemEffect
{
    [SerializeField] int addedChance;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceThatEnemyDontMix += addedChance;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceThatEnemyDontMix -= addedChance;
    }

    public override bool CanBeSpawned()
    {
        return GetPlayer().GetComponent<PlayerStats>().chanceThatEnemyDontMix < 100;
    }
}
