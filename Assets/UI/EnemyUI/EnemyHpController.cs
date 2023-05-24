using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpController : MonoBehaviour
{
    // The UI component for the enemy health bar.
    public EnemyStats enemy;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider healthSubSlider;
    [SerializeField] float subBarRate;
    
    private void Start() {
        gameObject.SetActive(true);
        enemy.onHealthChanged +=  HpChanged;
        enemy.onEnemyDeath += EnemyDied;
        
        var sliders = GetComponents<Slider>();

        healthSlider.maxValue = enemy.GetHealth();
        healthSubSlider.maxValue = enemy.GetHealth();
        healthSlider.value = enemy.GetHealth();
        healthSubSlider.value = enemy.GetHealth();
        healthSlider.gameObject.SetActive(false);
        healthSubSlider.gameObject.SetActive(false);
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        healthSlider.value = newHp;
        healthSlider.gameObject.SetActive(true);
        healthSubSlider.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (healthSubSlider.value == healthSlider.value) return;

        if (healthSubSlider.value < healthSlider.value)
        {
            healthSubSlider.value = healthSlider.value;
            return;
        }
        healthSubSlider.value -= Math.Min(healthSubSlider.value - healthSlider.value, subBarRate * Time.deltaTime);
    }

    /// <summary>
    /// When enemy dies, hide the health bar.
    /// </summary>
    private void EnemyDied() {
        gameObject.SetActive(false);
    }
}
