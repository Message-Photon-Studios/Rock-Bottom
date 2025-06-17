using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpController : MonoBehaviour
{
    // The UI component for the enemy health bar.
    public EnemyStats enemy;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider healthSubSlider;
    [SerializeField] Slider rainbowSlider;
    [SerializeField] float subBarRate;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float healthBarScale;
    [SerializeField] float healthBarBaseSize;
    
    private void Start() {
        gameObject.SetActive(true);
        enemy.onHealthChanged +=  HpChanged;
        enemy.onEnemyDeath += EnemyDied;
        enemy.onMaxHealthChanged += MaxHealthChanged;
        enemy.onColorChanged += EnemyColorChanged;
        var sliders = GetComponents<Slider>();

        healthSlider.maxValue = enemy.GetHealth();
        healthSubSlider.maxValue = enemy.GetHealth();
        healthSlider.value = enemy.GetHealth();
        healthSubSlider.value = enemy.GetHealth();
        rainbowSlider.value = 0;
        healthSlider.gameObject.SetActive(false);
        healthSubSlider.gameObject.SetActive(false);
        rainbowSlider.gameObject.SetActive(false);

        if (rectTransform)
            rectTransform.sizeDelta = new Vector2(healthBarBaseSize + healthBarScale * enemy.GetHealth(), rectTransform.sizeDelta.y);
    }

    void OnDestroy()
    {
        enemy.onHealthChanged -= HpChanged;
        enemy.onEnemyDeath -= EnemyDied;
        enemy.onMaxHealthChanged -= MaxHealthChanged;
        enemy.onColorChanged -= EnemyColorChanged;
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp)
    {
        healthSlider.value = newHp;
        healthSlider.gameObject.SetActive(true);
        healthSubSlider.gameObject.SetActive(true);
        rainbowSlider.gameObject.SetActive(true);
    }

    private void EnemyColorChanged(GameColor color)
    {
        if (enemy.IsRaibowed())
        {
            rainbowSlider.value = Player.instance.stats.rainbowExecutePercentage;
        }
        else
        {
            rainbowSlider.value = 0;
        }
    }

    /// <summary>
    /// Updates when the enemy's max health changes
    /// </summary>
    /// <param name="maxHealth"></param>
    /// <param name="newHp"></param>
    private void MaxHealthChanged(float maxHealth, float newHp)
    {
        healthSlider.maxValue = maxHealth;
        healthSubSlider.maxValue = maxHealth;
        healthSlider.value = newHp;
        healthSubSlider.value = newHp;
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
    private void EnemyDied(EnemyStats deadEnemy) {
        gameObject.SetActive(false);
    }
}
