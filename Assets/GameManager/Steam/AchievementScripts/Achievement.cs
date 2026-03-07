using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Achievement : MonoBehaviour
{
    [SerializeField] private string achievementId;
    [SerializeField] private string progressStat;

    protected virtual void Start()
    {
    }

    protected void RewardAchievement()
    {
        AchievementsManager.instance.RewardAchievement(achievementId);
        //Debug.Log("Achievement Rewarded! : " + achievementId);
    }

    public virtual void ProgressAchievement()
    {
        AchievementsManager.instance.ProgressAchievement(achievementId, progressStat);
    }

    public string GetAchivementId()
    {
        return achievementId;
    }
}
