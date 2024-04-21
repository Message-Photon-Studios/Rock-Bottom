using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardAchievementOnLoad : MonoBehaviour
{
    [SerializeField] string achievementId;
    private void Start() {
        AchivementsManager.instance.RewardAchievementsInstantly(achievementId);
    }
}
