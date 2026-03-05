using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAchievement : Achievement
{
    [SerializeField] Item item;
    [SerializeField] int itemAmount = 1;

    protected override void Start()
    {
        base.Start();
        Player.instance.playerInventory.onItemPickedUpOrRemoved += ItemsChanged;
    }

    void OnDisable()
    {
        Player.instance.playerInventory.onItemPickedUpOrRemoved -= ItemsChanged;
    }

    void ItemsChanged(Item itemChanged)
    {
        if(itemChanged == item)
        {
            if(Player.instance.playerInventory.getItemAmoutWithName(item.GetName())>= itemAmount)
            {
                RewardAchievement();
            }
        }
    }
}
