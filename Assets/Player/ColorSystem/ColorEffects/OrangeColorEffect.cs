using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/OrangeColorEffect")]
public class OrangeColorEffect : ColorEffect
{
    [SerializeField] int damageOverTime;
    [SerializeField] float time;
    [SerializeField] float effectRange;
    [SerializeField] GameObject floorFlames;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {
        int flames = (int) ((EffectFunction(power)+0.05) * 10)-6;
        enemyObj.GetComponent<EnemyStats>().BurnDamage(Mathf.RoundToInt(damageOverTime*power), time, effectRange, particles, floorFlames, true, flames);
        enemyObj.GetComponent<EnemyStats>().DamageEnemy(Mathf.RoundToInt(damage*power)+extraDamage);
    }
}
