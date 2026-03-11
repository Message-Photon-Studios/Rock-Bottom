using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
public class TheLongPathAchievement : BeatGameAchievement
{
    [Scene][SerializeField] string[] levelsNeeded;

    List<string> currentVisited = new List<string>();

    protected override void NewRunStarted()
    {
        base.NewRunStarted();
        currentVisited = new List<string>();
    }

    protected override void LevelLoaded()
    {
        base.LevelLoaded();
        string level = SceneManager.GetActiveScene().name;
        
        if(!currentVisited.Contains(level)) currentVisited.Add(level);
    }

    protected override void BossDefeated()
    {
        foreach(string level in levelsNeeded)
        {
            if(!currentVisited.Contains(level)) return;
        }

        RewardAchievement();
    }
}
