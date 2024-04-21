using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchievementsManager : MonoBehaviour
{
    Dictionary<string, Achievement> availableAchievements = new Dictionary<string, Achievement>();
    public static AchievementsManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            this.enabled = false;
        }
    }

    void Start()
    {
        Achievement[] achievementPrefabs = Resources.LoadAll<Achievement>("Achievements");
        foreach (Achievement achievement in achievementPrefabs)
        {
            GameObject obj = GameObject.Instantiate(achievement.gameObject, transform);
            availableAchievements.Add(achievement.GetAchivementId(), obj.GetComponent<Achievement>());
        }
    }

    /// <summary>
    /// Make progress on the specified achievement
    /// </summary>
    /// <param name="achievementId"></param>
    public void ProgressAchievement(string achievementId)
    {
        if(!availableAchievements.ContainsKey(achievementId)) return;
        availableAchievements[achievementId].ProgressAchievement();
    }

    /// <summary>
    /// Reward the specified achievement
    /// </summary>
    /// <param name="achievementId"></param>
    public void RewardAchievement(string achievementId)
    {
        if(!SteamManager.Initialized) return;

        Debug.Log("Achievement unlocked " + achievementId); 
        SteamUserStats.SetAchievement(achievementId);
        SteamUserStats.StoreStats();
    }
}
