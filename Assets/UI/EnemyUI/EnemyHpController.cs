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
    [SerializeField] Slider rainbowIndicator;
    [SerializeField] float subBarRate;

    PlayerStats playerStats;
    PlayerCombatSystem playerCombat;
    
    private void Start() {
        gameObject.SetActive(true);
        enemy.onHealthChanged +=  HpChanged;
        enemy.onEnemyDeath += EnemyDied;
        enemy.onColorChanged += SetRainbowIndicator;
        
        var sliders = GetComponents<Slider>();

        healthSlider.maxValue = enemy.GetHealth();
        healthSubSlider.maxValue = enemy.GetHealth();
        healthSlider.value = enemy.GetHealth();
        healthSubSlider.value = enemy.GetHealth();
        rainbowIndicator.maxValue = enemy.GetHealth();
        healthSlider.gameObject.SetActive(false);
        healthSubSlider.gameObject.SetActive(false);
        rainbowIndicator.gameObject.SetActive(false);
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatSystem>();
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        healthSlider.value = newHp;
        healthSlider.gameObject.SetActive(true);
        healthSubSlider.gameObject.SetActive(true);
        rainbowIndicator.gameObject.SetActive(true);
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

    private void SetRainbowIndicator(GameColor color)
    {
        if (color == null)
        {
            rainbowIndicator.value = 0;
            return;
        }
        if (color.name.Equals("Rainbow"))
        {
            rainbowIndicator.value = (int)(playerCombat.rainbowComboDamage * playerStats.colorRainbowMaxedPower);
            return;
        }
        rainbowIndicator.value = 0;
        return;
    }
}
