using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressRunAchievement : Achievement
{
    [SerializeField] int progressGoal;
    int progress = 0;
    protected override void Start()
    {
        base.Start();
        playerStats.onPlayerDied += ResetAchievement;
    }

    void ResetAchievement()
    {
        progress = 0;
    }

    void OnDisable()
    {
        playerStats.onPlayerDied -= ResetAchievement;
    }

    public override void ProgressAchievement()
    {
        progress ++;
        if(progress >= progressGoal) RewardAchievement();
    }
}
