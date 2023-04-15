using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/OrangeColorEffect")]
public class OrangeColorEffect : ColorEffect
{
    [SerializeField] float damageOverTime;
    [SerializeField] float time;
    [SerializeField] float effectRange;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        enemyObj.GetComponent<EnemyStats>().BurnDamage(damageOverTime*power, time, effectRange, particles);
        enemyObj.GetComponent<EnemyStats>().DamageEnemy(damage*power);
    }
}
