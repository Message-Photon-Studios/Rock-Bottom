using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The blue color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BlueColorEffect")]
public class BlueColorEffect : ColorEffect
{
    [SerializeField] float slow;
    [SerializeField] float duration;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        enemy.ChangeSpeed(enemy.GetSpeed() / slow, duration*power);
        enemy.DamageEnemy(damage*power);
    }
}
