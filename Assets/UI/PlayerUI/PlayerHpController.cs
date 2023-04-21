using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpController : MonoBehaviour
{
    // The players stats, used to access relevant UnityActions.
    PlayerStats playerStats;
    // The UI component for the health bar.
    [SerializeField] Slider healthSlider;

    private void OnEnable() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerStats.onMaxHealthChanged += MaxHpChanged;
        playerStats.onHealthChanged += HpChanged;
        playerStats.onPlayerDied += PlayerDied;
        gameObject.SetActive(true);
        MaxHpChanged(playerStats.GetMaxHealth());
        HpChanged(playerStats.GetHealth());
    }

    private void OnDisable() {
        playerStats.onMaxHealthChanged -= MaxHpChanged;
        playerStats.onHealthChanged -= HpChanged;
        playerStats.onPlayerDied -= PlayerDied;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When max hp is changed, update sliders max value.
    /// If current health is higher than max health, also update current value.
    /// </summary>
    /// <param name="newMaxHp"></param> Float with new max hp.
    private void MaxHpChanged(float newMaxHp) {
        healthSlider.maxValue = newMaxHp;
        if(healthSlider.maxValue < healthSlider.value) {
            healthSlider.value = newMaxHp;
        }
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        healthSlider.value = newHp;
    }

    /// <summary>
    /// When player dies, hide the health bar.
    /// </summary>
    private void PlayerDied() {
        gameObject.SetActive(false);
    }
}
