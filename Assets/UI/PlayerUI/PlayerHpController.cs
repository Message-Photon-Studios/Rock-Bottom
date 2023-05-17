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
    [SerializeField] Slider secondarySlider;
    [SerializeField] AnimationCurve maxHpChangeCurve;
    [SerializeField] float secondaryRate;
 
    private RectTransform rect;
    private RectTransform secondaryRect;
    private float origSize;
    private float sizeMultiplier = 1;

    private float maxHealth = 100;
    private float healthMultiplier = 1;
    private float targetValue = 100;
    private float movingValue = 0;

    private void OnEnable() {
        rect = healthSlider.GetComponent<RectTransform>();
        secondaryRect = secondarySlider.GetComponent<RectTransform>();
        origSize = rect.rect.width;

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerStats.onMaxHealthChanged += MaxHpChanged;
        playerStats.onHealthChanged += HpChanged;
        playerStats.onPlayerDied += PlayerDied;
        gameObject.SetActive(true);
        healthSlider.value = 0;

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
    private void MaxHpChanged(float newMaxHp)
    {
        StartCoroutine(IncreaseHealthBar(newMaxHp > maxHealth));

        maxHealth = newMaxHp;
        healthMultiplier = 100 / maxHealth;
        if(maxHealth < healthSlider.value) {
            healthSlider.value = 100;
            secondarySlider.value = 100;
            targetValue = 100; 
            movingValue = 100;
        }
    }

    /// <summary>
    /// When hp is changed, update slider;
    /// </summary>
    /// <param name="newHp"></param> Float with value to update. 
    private void HpChanged(float newHp) {
        targetValue = newHp * healthMultiplier;
    }

    /// <summary>
    /// When player dies, hide the health bar.
    /// </summary>
    private void PlayerDied() {
        gameObject.SetActive(false);
    }

    private IEnumerator IncreaseHealthBar(bool increased)
    {
        float newSizeMultiplier;
        if (increased)
            newSizeMultiplier = Math.Min(sizeMultiplier + (2.5f - sizeMultiplier) / 1.5f, sizeMultiplier + 0.2f);
        else
            newSizeMultiplier = Math.Max(sizeMultiplier + (sizeMultiplier - 1) / 1.5f, sizeMultiplier - 0.2f);

        var tempMult = sizeMultiplier;
        sizeMultiplier = newSizeMultiplier;

        for (var i = 0.0f; i < 1.0f; i += 0.02f)
        {
            var val = maxHpChangeCurve.Evaluate(i);
            float value = tempMult + ((newSizeMultiplier - tempMult) * val);
            Vector2 size = new Vector2(origSize * value, rect.rect.height);
            rect.sizeDelta = size;
            secondaryRect.sizeDelta = size;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void Update()
    {
        if (targetValue > movingValue)
        {
            secondarySlider.value = targetValue;
            healthSlider.value = Math.Min(healthSlider.value + Time.deltaTime * secondaryRate, targetValue);
            movingValue = healthSlider.value;
        }
        else if (targetValue < movingValue)
        {
            healthSlider.value = targetValue;
            secondarySlider.value = Math.Max(secondarySlider.value - Time.deltaTime * secondaryRate, targetValue);
            movingValue = secondarySlider.value;
        }
    }
}
