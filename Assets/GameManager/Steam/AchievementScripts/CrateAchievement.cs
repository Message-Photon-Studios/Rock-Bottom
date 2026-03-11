using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateAchievement : Achievement
{
    [SerializeField] int cratesNeeded = 1;

    private int currentOpened = 0;
    protected override void Start()
    {
        base.Start();
        AchievementsManager.instance.onWillowCrateOpened += WillowCrateOpened;
        GameManager.instance.onStartedNewRun += NewRun;
    }

    void OnDisable()
    {
        AchievementsManager.instance.onWillowCrateOpened -= WillowCrateOpened;
        GameManager.instance.onStartedNewRun -= NewRun;
    }

    void NewRun()
    {
        currentOpened = 0;
    }

    void WillowCrateOpened()
    {
        currentOpened += 1;
        if (currentOpened >= cratesNeeded) RewardAchievement();
    }
}
