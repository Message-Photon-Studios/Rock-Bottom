using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script handles the hit of the default attack
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerDefaultAttack : MonoBehaviour
{
    List<GameObject> targetedEnemies = new List<GameObject>();

    /// <summary>
    /// This action is called when the player hits an with the default attack
    /// </summary>
    public UnityAction<GameObject> onDefaultHit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && !targetedEnemies.Contains(other.gameObject))
        {
            targetedEnemies.Add(other.gameObject);
        }   
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && targetedEnemies.Contains(other.gameObject))
        {
            targetedEnemies.Remove(other.gameObject);
        }   
    }

    public void HitEnemies()
    {
        GameObject target = null;
        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            if(targetedEnemies[i] == null)
            {
                targetedEnemies.RemoveAt(i);
                i--;
                continue;
            }
            
            if(target == null || (target.GetComponent<EnemyStats>().GetColor() == null && targetedEnemies[i].GetComponent<EnemyStats>().GetColor() != null))
            {
                target = targetedEnemies[i];
                continue;
            }

            float mD = targetedEnemies[i].transform.position.x - transform.parent.transform.position.x;
            float tD = target.transform.position.x - transform.parent.transform.position.x;
            if(tD*tD > mD*mD)
                target = targetedEnemies[i];
        }


        if(target != null)
            onDefaultHit?.Invoke(target);
    }
}
