using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the player stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float hitInvincibilityTime;
    [SerializeField] GameManager gameManager;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement movement;
    float maxHealth;
    float invincibilityTimer = 0;

    /// <summary>
    /// This event fires when the player health is changed. The float is the new health.
    /// </summary>
    public UnityAction<float> onHealthChanged;
    
    /// <summary>
    /// This event fires when the players max health is set or changed. The float is the new max health
    /// </summary>
    public UnityAction<float> onMaxHealthChanged;

    /// <summary>
    /// The player died
    /// </summary>
    public UnityAction onPlayerDied;

    void OnEnable()
    {
        //TODO: Check so this doesnt cause a problem when changing scene.
        maxHealth = health;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
    }

    void Update()
    {
        if(invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if(invincibilityTimer < 0)
            {
                invincibilityTimer = 0;
                Physics2D.IgnoreLayerCollision(3,6,false);
            }
                
        }
    }

    /// <summary>
    /// Damage the player
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(float damage)
    {
        if(invincibilityTimer > 0) return;
        Physics2D.IgnoreLayerCollision(3,6);
        health -= damage;
        invincibilityTimer = hitInvincibilityTime;
        if(health <= 0)
        {
            animator.SetBool("dead", true);
            movement.movementRoot.SetTotalRoot("dead", true);
            invincibilityTimer = 3f;
        }
        animator.SetTrigger("damaged");
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    /// <param name="healing"></param>
    public void HealPlayer (float healing) 
    {
        health += healing;
        if(health > maxHealth) health = maxHealth;
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Returns the players current health
    /// </summary>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns the players max health. 
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Adds health points to the players max health and also heals the player the same ammount
    /// </summary>
    /// <param name="addMaxHealth"></param>
    public void AddMaxHealth(float addMaxHealth)
    {
        maxHealth += addMaxHealth;
        health += addMaxHealth;
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Kill the player
    /// </summary>
    public void KillPlayer()
    {
        //TODO
        Debug.Log("Player died. Player deaths not implemented");
        onPlayerDied?.Invoke();
        
        gameManager?.EndLevel();
    }

    /// <summary>
    /// Returns true if the player is invincible
    /// </summary>
    /// <returns></returns>
    public bool IsInvincible()
    {
        return invincibilityTimer > 0;
    }
}
