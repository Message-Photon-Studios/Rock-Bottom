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
        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            if(targetedEnemies[i] == null)
            {
                targetedEnemies.RemoveAt(i);
                i--;
                continue;
            }
            onDefaultHit?.Invoke(targetedEnemies[i]);
        }
    }
}
