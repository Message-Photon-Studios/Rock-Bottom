using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcentradedColorBuff : ItemEffect
{
    [SerializeField] float smallBuff;
    [SerializeField] float midBuff;
    [SerializeField] float maxBuff;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<ColorInventory>().AddConcentratedColorBuffs(smallBuff, midBuff, maxBuff);
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<ColorInventory>().AddConcentratedColorBuffs(-smallBuff, -midBuff, -maxBuff);
    }

}
