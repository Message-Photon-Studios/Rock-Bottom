using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpController : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Slider healthSlider;

    private void OnEnable() {
        playerStats.onMaxHealthChanged += MaxHpChanged;
        playerStats.onHealthChanged += HpChanged;
        playerStats.onPlayerDied += PlayerDied;
        gameObject.SetActive(true);
    }

    private void OnDisable() {
        playerStats.onMaxHealthChanged -= MaxHpChanged;
        playerStats.onHealthChanged -= HpChanged;
        playerStats.onPlayerDied -= PlayerDied;
        gameObject.SetActive(false);
    }

    private void MaxHpChanged(float newMaxHp) {
        healthSlider.maxValue = newMaxHp;
        if(healthSlider.maxValue < healthSlider.value) {
            healthSlider.value = newMaxHp;
        }
    }

    private void HpChanged(float newHp) {
        healthSlider.value = newHp;
    }

    private void PlayerDied() {
        gameObject.SetActive(false);
    }
}
