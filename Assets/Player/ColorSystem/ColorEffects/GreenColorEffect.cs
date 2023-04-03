using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The green color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/GreenColorEffect")]
public class GreenColorEffect : ColorEffect
{
    [SerializeField] float damageOverTime;
    [SerializeField] float time;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        enemy.DamageOverTime(damageOverTime * power, time);
        enemy.DamageEnemy(damage);
    }
}
