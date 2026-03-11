using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunAchievement : BeatGameAchievement
{
    [SerializeField] float maxTime;

    protected override void BossDefeated()
    {
        if(maxTime >= GameManager.instance.currentRunTime) RewardAchievement();
    }
}
