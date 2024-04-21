using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchivementsManager : MonoBehaviour
{
    [SerializeField] List<string> availableAchievements;
    public static AchivementsManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            this.enabled = false;
        }
    }

    public void RewardAchievementsInstantly(string achievement)
    {
        if(!SteamManager.Initialized) return;
        if(!availableAchievements.Contains(achievement)) return;
        
        SteamUserStats.SetAchievement(achievement);
        SteamUserStats.StoreStats();
    }
}
