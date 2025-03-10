using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The grey color effect for empty bottles
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/GreyColorEffect")]
public class GreyColorEffect : ColorEffect
{
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power)+extraDamage);
    }
}
