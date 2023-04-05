using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float hitInvincibilityTime;
    float maxHealth;
    float invincibilityTimer = 0;

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        maxHealth = health;
    }

    void Update()
    {
        if(invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if(invincibilityTimer < 0)
                invincibilityTimer = 0;
        }
    }

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(float damage)
    {
        if(invincibilityTimer > 0) return;
        health -= damage;
        invincibilityTimer = hitInvincibilityTime;
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
