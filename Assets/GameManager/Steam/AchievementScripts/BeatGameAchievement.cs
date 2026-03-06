using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BeatGameAchievement : Achievement
{
    protected override void Start()
    {
        base.Start();
        GameManager.instance.onLevelLoaded += Setup;
        GameManager.instance.onStartedNewRun += NewRunStarted;
        GameManager.instance.onGameWon += BossDefeated;
    }

    protected virtual void OnDisable()
    {
        GameManager.instance.onGameWon -= BossDefeated;
        GameManager.instance.onLevelLoaded -= LevelLoaded;
        GameManager.instance.onStartedNewRun -= NewRunStarted;
    }

    private void Setup()
    {
        LevelLoaded();
    }

    protected virtual void NewRunStarted(){}

    protected virtual void LevelLoaded(){}
    
    protected abstract void BossDefeated();
}
