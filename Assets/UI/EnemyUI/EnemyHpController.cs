using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpController : MonoBehaviour
{
    // The UI component for the enemy health bar.
    [SerializeField] Slider healthSliderLeft;
    [SerializeField] Slider healthSliderRight;
    [SerializeField] EnemyStats enemy;

    private void OnEnable() {
        gameObject.SetActive(true);
        enemy.onHealthChanged +=  HpChanged;
        enemy.onEnemyDeath += EnemyDied;

        healthSliderLeft.maxValue = enemy.GetHealth();
        healthSliderRight.maxValue = enemy.GetHealth();

        healthSliderLeft.gameObject.SetActive(false);
        healthSliderRight.gameObject.SetActive(false);
    }

    private void OnDisable() {
        gameObject.SetActive(false);
        enemy.onHealthChanged -=  HpChanged;
        enemy.onEnemyDeath -= EnemyDied;
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        healthSliderLeft.value = newHp;
        healthSliderRight.value = newHp;

        healthSliderLeft.gameObject.SetActive(true);
        healthSliderRight.gameObject.SetActive(true);
    }

    /// <summary>
    /// When enemy dies, hide the health bar.
    /// </summary>
    private void EnemyDied() {
        gameObject.SetActive(false);
    }
}
