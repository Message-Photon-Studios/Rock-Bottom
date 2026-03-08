using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloopAchivement : BeatGameAchievement
{
    [SerializeField] int loopsNeeded = 1;
    protected override void BossDefeated()
    {
        if(GameManager.instance.rerunNum >= loopsNeeded) RewardAchievement();
    }
}
