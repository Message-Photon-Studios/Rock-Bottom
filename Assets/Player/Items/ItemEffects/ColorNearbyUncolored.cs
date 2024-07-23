using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorNearbyUncolored : ItemEffect
{
    [SerializeField] int chanceToColorNearby;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceToColorNearby += chanceToColorNearby;
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerStats>().chanceToColorNearby -= chanceToColorNearby;
    }

    public override bool CanBeSpawned()
    {
        return GetPlayer().GetComponent<PlayerStats>().chanceToColorNearby < 100;
    }
}
