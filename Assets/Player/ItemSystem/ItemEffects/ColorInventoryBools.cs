using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInventoryBools : ItemEffect
{
    [SerializeField] bool dontMixColor;
    [SerializeField] bool autoRotate;
    [SerializeField] bool chaosBottle;
    [SerializeField] bool routedSheild;
    [SerializeField] bool chaoticMixer;
    public override void ActivateEffect()
    {
        ColorInventory colorInv = GetPlayer().GetComponent<ColorInventory>();
        if (dontMixColor) colorInv.dontMixColor = true;
        if (autoRotate) colorInv.autoRotate = true;
        if (chaosBottle) colorInv.chaosEnabled = true;
        if (routedSheild) colorInv.routedSheild = true;
        if (chaoticMixer) EnemyStats.chaoticMixer = true;

    }

    public override void DisableEffect()
    {
        ColorInventory colorInv = GetPlayer().GetComponent<ColorInventory>();
        if (dontMixColor) colorInv.dontMixColor = false;
        if (autoRotate) colorInv.autoRotate = false;
        if (chaosBottle) colorInv.chaosEnabled = false;
        if (routedSheild) colorInv.routedSheild = false;
        if (chaoticMixer) EnemyStats.chaoticMixer = false;
    }

}
