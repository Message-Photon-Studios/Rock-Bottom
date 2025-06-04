using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsController : BigMenu
{
    [SerializeField] MainMenuController controller;
    [SerializeField] Slider screenSizeSlider;
    [SerializeField] TMP_Text screenSizeText;

    void Start()
    {
        if (PlayerPrefs.HasKey(SettingsManager.screenSizeKey))
        {
            float screenSize = PlayerPrefs.GetFloat(SettingsManager.screenSizeKey);
            screenSizeSlider.value = screenSize;
            screenSizeText.text = screenSize.ToString("F1");
        }
        CloseMenu();
    }

    public void UpdateCameraSizeSlider()
    {
        SettingsManager.instance.SetCameraSize(screenSizeSlider.value);
        screenSizeText.text = screenSizeSlider.value.ToString("F1");
    }
}
