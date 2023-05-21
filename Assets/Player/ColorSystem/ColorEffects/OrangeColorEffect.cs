using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/OrangeColorEffect")]
public class OrangeColorEffect : ColorEffect
{
    [SerializeField] int damageOverTime;
    [SerializeField] float time;
    [SerializeField] float effectRange;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        enemyObj.GetComponent<EnemyStats>().BurnDamage(Mathf.RoundToInt(damageOverTime*power), time, effectRange, particles);
        enemyObj.GetComponent<EnemyStats>().DamageEnemy(Mathf.RoundToInt(damage*power));
    }
}
