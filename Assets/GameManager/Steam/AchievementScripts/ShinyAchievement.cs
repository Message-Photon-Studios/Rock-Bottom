using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinyAchievement : Achievement
{
    protected override void Start()
    {
        base.Start();
        GameManager.instance.onPetrifiedPigmentChanged += PickedUpPigment;
    }

    void OnDisable()
    {
        GameManager.instance.onPetrifiedPigmentChanged -= PickedUpPigment;
    }

    void PickedUpPigment(int pigment)
    {
        if(pigment > 0)
        {
            RewardAchievement();
        }
    }
}
