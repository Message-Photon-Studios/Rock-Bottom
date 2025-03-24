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
    [SerializeField] AnimationCurve maxTHpChangeCurve;
    [SerializeField] float secondaryRate;
    [SerializeField] RectTransform permanentTHPMarker;

    private RectTransform rect;
    private RectTransform secondaryRect;
    private float sizeMultiplier = 1;
    private float origSize;
    private float maxPTHp = 0;

    private float maxTHealth = 0;
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
        rect = healthSlider.GetComponent<RectTransform>();
        secondaryRect = secondarySlider.GetComponent<RectTransform>();
        origSize = rect.rect.width;

        playerStats.onShieldChanged += ShieldChanged;
        playerStats.onMaxShieldChanged += MaxShieldChanged;
        playerStats.onPlayerDied += PlayerDied;
        playerStats.onMaxPermanentShieldChanged += MaxPermanentShieldChanged;

        ShieldChanged(0);
        healthSliderValue = 0;
        MaxShieldChanged(playerStats.GetMaxShield());

        maxPTHp = (float)playerStats.GetMaxPermanentShield();
        float pthpX = GetComponent<RectTransform>().sizeDelta.x * (maxPTHp / (float)playerStats.GetMaxShield());
        Debug.Log("size = " + GetComponent<RectTransform>().sizeDelta.x  + " * " + ((float)playerStats.GetMaxPermanentShield()/(float)playerStats.GetMaxShield()) + " = " + pthpX);
        permanentTHPMarker.anchoredPosition = new Vector3(pthpX , 0, 0);
    }

    private void OnDisable() {
        playerStats.onShieldChanged -= ShieldChanged;
        playerStats.onMaxShieldChanged -= MaxShieldChanged;
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

    private void MaxShieldChanged(float newMaxTHp)
    {
        if (newMaxTHp == maxTHealth) return;
        if (maxTHealth != 0)
            StartCoroutine(IncreaseHealthBar(newMaxTHp > maxTHealth));

        maxTHealth = newMaxTHp;
        healthMultiplier = 50 / maxTHealth;
        healthSliderValue = healthSliderValue;
        if (maxTHealth < healthSliderValue)
        {
            healthSliderValue = 50;
            secondarySlider.value = 50;
            targetValue = 50;
            movingValue = 50;
        }
    }

    private void MaxPermanentShieldChanged(float newMaxPTHp)
    {
        maxPTHp = newMaxPTHp;
        float pthpX = GetComponent<RectTransform>().sizeDelta.x * (maxPTHp / (float)playerStats.GetMaxShield());
        Debug.Log("size = " + GetComponent<RectTransform>().sizeDelta.x + " * " + ((float)playerStats.GetMaxPermanentShield() / (float)playerStats.GetMaxShield()) + " = " + pthpX);
        permanentTHPMarker.anchoredPosition = new Vector3(pthpX, 0, 0);
        Debug.Log(newMaxPTHp);
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
            var val = maxTHpChangeCurve.Evaluate(i);
            float value = tempMult + ((newSizeMultiplier - tempMult) * val);
            Vector2 size = new Vector2(origSize * value, rect.rect.height);
            rect.sizeDelta = size;
            secondaryRect.sizeDelta = size;
            MaxPermanentShieldChanged(maxPTHp);
            yield return new WaitForSeconds(0.01f);
        }
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
