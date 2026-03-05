using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SacrificedAchievement : Achievement
{
    protected override void Start()
    {
        base.Start();
        AchievementsManager.instance.onDeathShrineUsed += RewardAchievement;
    }

    void OnDisable()
    {
        AchievementsManager.instance.onDeathShrineUsed -= RewardAchievement;
    }

}
