using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class PlayerHpController : MonoBehaviour
{
    // The players stats, used to access relevant UnityActions.
    PlayerStats playerStats;
    // The UI component for the health bar.
    [SerializeField] Slider healthSlider;
    [SerializeField] AnimationCurve maxHpChangeCurve;
 
    private RectTransform rect;
    private float origSize = 1;
    private float sizeMultiplier;

    private void OnEnable() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerStats.onMaxHealthChanged += MaxHpChanged;
        playerStats.onHealthChanged += HpChanged;
        playerStats.onPlayerDied += PlayerDied;
        gameObject.SetActive(true);
        //MaxHpChanged(playerStats.GetMaxHealth());
        HpChanged(playerStats.GetHealth());
        rect = gameObject.GetComponent<RectTransform>();
        origSize = rect.rect.width;
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
    private void MaxHpChanged(float newMaxHp)
    {
        if (sizeMultiplier == 0)
            sizeMultiplier = 1;
        else
            StartCoroutine(IncreaseHealthBar(newMaxHp));

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

    private IEnumerator IncreaseHealthBar(float newMaxHp)
    {
        float newSizeMultiplier;
        if (healthSlider.maxValue < newMaxHp)
            newSizeMultiplier = sizeMultiplier + (2 - sizeMultiplier) / 2.0f;
        else
            newSizeMultiplier = sizeMultiplier + (sizeMultiplier - 1) / 2.0f;

        for (var i = 0.0f; i < 1.0f; i += 0.02f)
        {
            var val = maxHpChangeCurve.Evaluate(i);
            float value = sizeMultiplier + ((newSizeMultiplier - sizeMultiplier) * val);
            rect.sizeDelta = new Vector2(origSize * value, rect.rect.height);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
