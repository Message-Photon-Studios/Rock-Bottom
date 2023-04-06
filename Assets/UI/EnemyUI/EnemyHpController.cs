using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpController : MonoBehaviour
{
    // The UI component for the enemy health bar.
    [SerializeField] Slider healthSliderLeft;
    [SerializeField] Slider healthSliderRight;

    private void OnEnable() {
        gameObject.SetActive(true);
    }

    private void OnDisable() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When max hp is changed, update sliders max value.
    /// If current health is higher than max health, also update current value.
    /// </summary>
    /// <param name="newMaxHp"></param> Float with new max hp.
    private void MaxHpChanged(float newMaxHp) {
        healthSliderLeft.maxValue = newMaxHp;
        healthSliderRight.maxValue = newMaxHp;
        IsAtFullhealth();
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        healthSliderLeft.value = newHp;
        healthSliderRight.value = newHp;
        IsAtFullhealth();
    }

    private void IsAtFullhealth() {
        if(healthSliderLeft.maxValue == healthSliderLeft.value) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// When enemy dies, hide the health bar.
    /// </summary>
    private void EnemyDied() {
        gameObject.SetActive(false);
    }
}
