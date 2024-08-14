using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBoostItem : ItemEffect
{
    [SerializeField] float coinBoost = 0;
    public override void ActivateEffect()
    {
        ItemInventory inv = GetPlayer().GetComponent<ItemInventory>();
        inv.AddCoinBoost(coinBoost);
    }

    public override void DisableEffect()
    {
        ItemInventory inv = GetPlayer().GetComponent<ItemInventory>();
        inv.AddCoinBoost(-coinBoost);
    }
}
