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
        {
            EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();

            GameColor comboColor = gameColor.MixColor(enemy.GetComboColor());

            enemy.SetComboColor(comboColor);

            if(comboColor.name == "Brown") enemy.SetComboColor(null);

            gameColor.colorEffect.Apply(other.gameObject, player, power*powerScale);
            if(comboColor != gameColor && enemy != null) comboColor.colorEffect.Apply(other.gameObject, player,power*powerScale);

        }
    }
}
