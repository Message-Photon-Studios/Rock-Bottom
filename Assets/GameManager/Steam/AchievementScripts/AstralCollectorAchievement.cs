using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstralCollectorAchievement : BeatGameAchievement
{
    [SerializeField] int coinsNeeded;

    protected override void BossDefeated()
    {
        if(playerStats.gameObject.GetComponent<ItemInventory>().GetCoins() >= coinsNeeded) RewardAchievement();
    }
}
