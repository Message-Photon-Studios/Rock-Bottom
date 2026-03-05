using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspiredAchievement : Achievement
{
    [SerializeField] int inspirationRequired = 1;
    protected override void Start()
    {
        base.Start();
        GameManager.instance.onInspirationChanged += Inspired;
    }

    void OnDisable()
    {
        GameManager.instance.onInspirationChanged -= Inspired;
    }

    void Inspired(int inspirationAdded)
    {
        if(GameManager.instance.GetInspiration() >= inspirationRequired)
        {
            RewardAchievement();
        }
    }
}
