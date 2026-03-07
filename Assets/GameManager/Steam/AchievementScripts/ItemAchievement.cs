using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAchievement : Achievement
{
    [SerializeField] Item item;
    [SerializeField] int itemAmount = 1;

    private int collected = 0;

    protected override void Start()
    {
        base.Start();
        Player.instance.playerInventory.onItemPickedUpOrRemoved += ItemsChanged;
        GameManager.instance.onStartedNewRun += RunStarted;
    }

    void RunStarted()
    {
        Player.instance.playerInventory.onItemPickedUpOrRemoved += ItemsChanged;
        collected = 0;
    }

    void OnDisable()
    {
        Player.instance.playerInventory.onItemPickedUpOrRemoved -= ItemsChanged;
        GameManager.instance.onStartedNewRun -= RunStarted;
    }

    void ItemsChanged(Item itemChanged)
    {
        if(itemChanged == item)
        {
            collected ++;
            if(collected >= itemAmount)
            {
                RewardAchievement();
            }
        }
    }
}
