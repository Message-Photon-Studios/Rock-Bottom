using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKilledRunAchievement : Achievement
{
    [SerializeField] int enemyKills;
    int kills = 0;
    protected override void Start()
    {
        base.Start();
        playerStats.onPlayerDied += ResetAchievement;
    }

    void ResetAchievement()
    {
        kills = 0;
    }

    void OnDisable()
    {
        playerStats.onPlayerDied -= ResetAchievement;
    }

    public override void ProgressAchievement()
    {
        kills ++;
        if(kills >= enemyKills) RewardAchievement();
    }
}
