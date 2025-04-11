using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyBrushBuff : ItemEffect
{
    [SerializeField] int minBuff;
    [SerializeField] int maxBuff;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().AddEmergencyDamage(minBuff, maxBuff);
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().AddEmergencyDamage(-minBuff, -maxBuff);
    }

}
