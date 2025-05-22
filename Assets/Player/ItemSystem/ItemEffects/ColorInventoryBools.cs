using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInventoryBools : ItemEffect
{
    [SerializeField] bool dontMixColor;
    [SerializeField] bool crackedUrn;
    [SerializeField] bool chaosBottle;
    [SerializeField] bool routedSheild;
    [SerializeField] bool chaoticMixer;
    [SerializeField] bool shatteredPrism;
    [SerializeField] bool centrifuge;
    [SerializeField] bool enemyDontMix;
    [SerializeField] bool enemyGiveColorOnChange;
    [SerializeField] bool enemyGiveColorOnSame;
    [SerializeField] bool doubleExtraDamage;
    [SerializeField] bool greatBrushFirstHit;
    public override void ActivateEffect()
    {
        ColorInventory colorInv = GetPlayer().GetComponent<ColorInventory>();
        if (dontMixColor) colorInv.dontMixColor = true;
        if (crackedUrn) colorInv.crackedUrn = true;
        if (chaosBottle) colorInv.chaosEnabled = true;
        if (routedSheild) colorInv.routedSheild = true;
        if (chaoticMixer) EnemyStats.chaoticMixer = true;
        if (shatteredPrism) colorInv.shatteredPrism = true;
        if (centrifuge) colorInv.centrifuge = true;
        if (enemyDontMix) colorInv.enemyDontMix = true;
        if (enemyGiveColorOnChange) colorInv.enemyGiveColorOnChange = true;
        if (enemyGiveColorOnSame) colorInv.enemyGiveColorOnSame = true;
        if (doubleExtraDamage) colorInv.doubleExtraDamage = true;
        if (greatBrushFirstHit) colorInv.greatBrushFirstHit = true;

    }

    public override void DisableEffect()
    {
        ColorInventory colorInv = GetPlayer().GetComponent<ColorInventory>();
        if (dontMixColor) colorInv.dontMixColor = false;
        if (crackedUrn) colorInv.crackedUrn = false;
        if (chaosBottle) colorInv.chaosEnabled = false;
        if (routedSheild) colorInv.routedSheild = false;
        if (chaoticMixer) EnemyStats.chaoticMixer = false;
        if (shatteredPrism) colorInv.shatteredPrism = false;
        if (centrifuge) colorInv.centrifuge = false;
        if (enemyDontMix) colorInv.enemyDontMix = false;
        if (enemyGiveColorOnChange) colorInv.enemyGiveColorOnChange = false;
        if (enemyGiveColorOnSame) colorInv.enemyGiveColorOnSame = false;
        if (doubleExtraDamage) colorInv.doubleExtraDamage = false;
        if (greatBrushFirstHit) colorInv.greatBrushFirstHit = true;
    }

}
