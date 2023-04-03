using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Gameplay Color/Color Effect/YellowColorEffect")]
public class YellowColorEffect : ColorEffect
{
    [SerializeField] float effectRange;
    public override void Apply(GameObject enemyObj, GameObject playerObj, float power)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objs)
        {
            if((obj.transform.position - enemyObj.transform.position).sqrMagnitude < effectRange)
            {
                obj.GetComponent<EnemyStats>().DamageEnemy(damage*power);
            }
        }
    }
}
