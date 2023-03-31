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
    /// <summary>
    /// This action is called when the player hits an with the default attack
    /// </summary>
    public UnityAction<GameObject> onDefaultHit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            onDefaultHit?.Invoke(other.gameObject);

            gameObject.SetActive(false);
        }   
    }
}
