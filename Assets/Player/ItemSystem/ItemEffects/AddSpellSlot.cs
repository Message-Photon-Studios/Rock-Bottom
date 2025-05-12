using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpellSlot : ItemEffect
{
    public override void ActivateEffect()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();
        if(inv.colorSlots.Count < 4)
            inv.AddColorSlot();
    }

    public override void DisableEffect()
    {
        //You cant  remove this effect currently.
    }

    public override bool CanBeSpawned()
    {
        ColorInventory inv = GetPlayer().GetComponent<ColorInventory>();

        int slotsInWolrd = ItemSpellManager.instance.GetEffectsInLevel<AddSpellSlot>(this).Count;

        return inv.colorSlots.Count + slotsInWolrd < 4;
    }
}
