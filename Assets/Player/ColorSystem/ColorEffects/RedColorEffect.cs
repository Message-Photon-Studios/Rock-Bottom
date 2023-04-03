using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The red color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/RedColorEffect")]
public class RedColorEffect : ColorEffect
{
    [SerializeField] float healing;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats player = playerObj.GetComponent<PlayerStats>();

        enemy.DamageEnemy(damage*power);
        player.HealPlayer(healing*power);
    }
}
