using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects all the impacted enemies with the color effect
/// </summary>
public class ColorSpellImpact : ColorSpell
{
    
    protected override void Impact(Collision2D other)
    {
        if(other.collider.CompareTag("Enemy"))
            colorEffect.Apply(other.gameObject, player,power*powerScale);
    }
}
