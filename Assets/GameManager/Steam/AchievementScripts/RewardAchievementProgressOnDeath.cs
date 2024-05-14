using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class RewardAchievementProgressOnDeath : MonoBehaviour
{
    [SerializeField] Achievement achievementObject;
    void OnEnable()
    {
        GetComponent<EnemyStats>().onEnemyDeath += EnemyDied;
    }

    void OnDisable()
    {
       GetComponent<EnemyStats>().onEnemyDeath -= EnemyDied; 
    }

    void EnemyDied()
    {
        AchievementsManager.instance.ProgressAchievement(achievementObject.GetAchivementId());
    }
}
