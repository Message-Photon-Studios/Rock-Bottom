using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Achievement : MonoBehaviour
{
    [SerializeField] private string achievementId;
    protected PlayerStats playerStats;

    protected virtual void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    protected void RewardAchievement()
    {
        AchievementsManager.instance.RewardAchievement(achievementId);
    }

    public virtual void ProgressAchievement()
    {

    }

    public string GetAchivementId()
    {
        return achievementId;
    }
}
