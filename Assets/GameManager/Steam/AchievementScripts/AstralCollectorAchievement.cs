using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstralCollectorAchievement : Achievement
{
    [SerializeField] int coinsNeeded;
    protected override void Start()
    {
        base.Start();
        BossEnemyController.onBossDefeated += BossDefeated;
    }

    void OnDisable()
    {
        BossEnemyController.onBossDefeated -= BossDefeated;
    }

    void BossDefeated()
    {
        if(playerStats.gameObject.GetComponent<ItemInventory>().GetCoins() >= coinsNeeded) RewardAchievement();
    }
}
