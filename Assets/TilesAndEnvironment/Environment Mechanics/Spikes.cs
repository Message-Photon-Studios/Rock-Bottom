using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Spikes : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float damageTimer = 1f;
    float damageTime = 0;
    bool playerInSpikes = false;
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    void Start()
    {
        playerStats = PlayerLevelMananger.instance.playerStats;
        playerMovement = PlayerLevelMananger.instance.playerMovement;
    }

    void Update()
    {
        if(playerInSpikes)
        {
            damageTime -= Time.deltaTime;
            if(damageTime < 0)
            {
                damageTime = damageTimer;
                playerStats.DamagePlayer(damage, null);
                playerMovement.movementRoot.SetRoot("spikes", .2f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            damageTime = 0;
            playerInSpikes = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInSpikes = false;
        }
    }
}
