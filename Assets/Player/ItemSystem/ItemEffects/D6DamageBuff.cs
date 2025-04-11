using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D6DamageBuff : ItemEffect
{
    [SerializeField] int addD6 = 1;
    public override void ActivateEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().AddD6(addD6);
    }

    public override void DisableEffect()
    {
        GetPlayer().GetComponent<PlayerCombatSystem>().AddD6(-addD6);
    }

}
