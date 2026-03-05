using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalExpertAchievement : BeatGameAchievement
{
    bool healed = false;

    protected override void Start()
    {
        base.Start();
        AchievementsManager.instance.onHealingShrineUsed += HealthShrineUsed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        AchievementsManager.instance.onHealingShrineUsed -= HealthShrineUsed;
    }

    void HealthShrineUsed()
    {
        healed = true;
    }
    protected override void NewRunStarted()
    {
        healed = false;
    }
    protected override void BossDefeated()
    {
        if(healed == false) RewardAchievement();
    }
}