using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspiredAchievement : Achievement
{
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
        ProgressAchievement();
    }
}
