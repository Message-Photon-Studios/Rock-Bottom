using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The brown color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BrownColorEffect")]
public class BrownColorEffect : ColorEffect
{
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        enemy.DamageEnemy(damage*power);
    }
}
