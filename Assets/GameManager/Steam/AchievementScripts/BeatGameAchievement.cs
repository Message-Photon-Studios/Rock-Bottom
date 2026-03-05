using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BeatGameAchievement : Achievement
{
    protected override void Start()
    {
        base.Start();
        BossEnemyController.onBossDefeated += BossDefeated;
        GameManager.instance.onLevelLoaded += LevelLoaded;
        GameManager.instance.onStartedNewRun += NewRunStarted;
    }

    protected virtual void OnDisable()
    {
        BossEnemyController.onBossDefeated -= BossDefeated;
        GameManager.instance.onLevelLoaded -= LevelLoaded;
        GameManager.instance.onStartedNewRun -= NewRunStarted;
    }

    protected virtual void NewRunStarted(){}

    protected virtual void LevelLoaded(){}
    
    protected abstract void BossDefeated();
}
