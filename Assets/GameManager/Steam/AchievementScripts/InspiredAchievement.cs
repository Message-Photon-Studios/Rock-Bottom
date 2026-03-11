using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspiredAchievement : Achievement
{
    protected override void Start()
    {
        base.Start();
        GameManager.instance.onSpellUnlocked += Inspired;
    }

    void OnDisable()
    {
        GameManager.instance.onSpellUnlocked -= Inspired;
    }

    void Inspired(ColorSpell spell)
    {
        ProgressAchievement();
    }
}
