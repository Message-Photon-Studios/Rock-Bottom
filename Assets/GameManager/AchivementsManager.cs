using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementsManager : MonoBehaviour
{
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
    }
}
