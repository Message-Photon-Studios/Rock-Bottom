using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpellSlot : ItemEffect
{
    public override void ActivateEffect()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        if(inv.colorSlots.Count < 5)
            inv.AddColorSlot();
    }

    public override void DisableEffect()
    {
        //You cant  remove this effect currently.
    }

    public override bool CanBeSpawned()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        return inv.colorSlots.Count < 5;
    }
}
