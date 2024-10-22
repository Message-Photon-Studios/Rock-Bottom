using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using Time = UnityEngine.Time;
public class PlayerShieldController : MonoBehaviour
{

    PlayerStats playerStats;
    // The UI component for the health bar.
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider secondarySlider;
    [SerializeField] float secondaryRate;

    private float healthMultiplier = 1;
    private float targetValue = 1;
    private float movingValue = 100;

    private float healthSliderValue
    {
        get => healthSlider.value;

        set
        {
            healthSlider.value = value;
        }
    }

    private void OnEnable() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
        playerStats.onShieldChanged += ShieldChanged;
        playerStats.onPlayerDied += PlayerDied;

        ShieldChanged(0);
        healthSliderValue = 0;
    }

    private void OnDisable() {
        playerStats.onShieldChanged -= ShieldChanged;
        playerStats.onPlayerDied -= PlayerDied;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void ShieldChanged(float newHp) {
        targetValue = newHp * healthMultiplier;
    }

    /// <summary>
    /// When player dies, hide the health bar.
    /// </summary>
    private void PlayerDied() {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (targetValue > movingValue)
        {
            secondarySlider.value = targetValue;
            healthSliderValue = Math.Min(healthSliderValue + Time.deltaTime * secondaryRate, targetValue);
            movingValue = healthSliderValue;
        }
        else if (targetValue < movingValue)
        {
            healthSliderValue = targetValue;
            secondarySlider.value = Math.Max(secondarySlider.value - Time.deltaTime * secondaryRate, targetValue);
            movingValue = secondarySlider.value;
        }
    }
}
