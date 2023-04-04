using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] float health = 100;
    float maxHealth;

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        maxHealth = health;
    }

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    /// <param name="healing"></param>
    public void HealPlayer (float healing) 
    {
        health += healing;
        if(health > maxHealth) health = maxHealth;
    }

    /// <summary>
    /// Kill the player
    /// </summary>
    public void KillPlayer()
    {
        //TODO
        Debug.Log("Player died. Player deaths not implemented");
    }
}
