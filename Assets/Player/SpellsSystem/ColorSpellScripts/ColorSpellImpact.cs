using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects all the impacted enemies with the color effect
/// </summary>
public class ColorSpellImpact : ColorSpell
{
    
    protected override void Impact(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();

            GameColor comboColor = gameColor.MixColor(enemy.GetComboColor());

            enemy.SetComboColor(comboColor);

            if(comboColor != gameColor)
            {
                enemy.currentCombo *= 2;
                if(enemy.currentCombo == 0) enemy.currentCombo = 1;

                if(comboColor.name == "Brown" ) 
                {
                    enemy.SetComboColor(null);
                    float comboDamage = player.GetComponent<PlayerCombatSystem>().comboBaseDamage * enemy.currentCombo;
                    enemy.currentCombo = 0;
                    enemy.DamageEnemy(comboDamage);
                }
            }


            if(enemy != null) gameColor.colorEffect.Apply(other.gameObject, player, power*powerScale);

        }
    }
}
