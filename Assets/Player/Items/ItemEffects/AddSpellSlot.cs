using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpellSlot : ItemEffect
{
    public override void ActivateEffect()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        inv.AddColorSlot();
    }

    public override bool CanBeSpawned()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        return inv.colorSlots.Count < 6;
    }
}