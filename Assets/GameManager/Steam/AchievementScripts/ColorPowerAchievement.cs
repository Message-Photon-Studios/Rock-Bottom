using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPowerAchievement : Achievement
{
    [SerializeField] float powerNeeded = 5;
    protected override void Start()
    {
        base.Start();
        Player.instance.playerCombatSystem.onSpellCast += SpellCast;
        GameManager.instance.onStartedNewRun += LevelLoaded;
    }

    void LevelLoaded()
    {
        Player.instance.playerCombatSystem.onSpellCast += SpellCast;
    }
    void OnDisable()
    {
        Player.instance.playerCombatSystem.onSpellCast -= SpellCast;
        GameManager.instance.onStartedNewRun -= LevelLoaded;
    }

    void SpellCast (float power, ColorSpell spell, GameColor color)
    {
        if(power >= powerNeeded) RewardAchievement();
    }
}
