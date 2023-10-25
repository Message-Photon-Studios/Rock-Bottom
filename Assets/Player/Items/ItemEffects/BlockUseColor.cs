using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUseColor : ItemEffect
{
    [SerializeField] private int blockDrainColor = 10;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<ColorInventory>().blockDrainColor += 10;
    }

    public override bool CanBeSpawned()
    {
        return GetPlayer().GetComponent<ColorInventory>().blockDrainColor < 50;
    }
}