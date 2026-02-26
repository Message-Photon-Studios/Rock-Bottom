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
    [SerializeField] Selectable firstOption;

    void Start()
    {
        if (PlayerPrefs.HasKey(SettingsManager.screenSizeKey))
        {
            float screenSize = PlayerPrefs.GetFloat(SettingsManager.screenSizeKey);
            screenSizeSlider.value = screenSize;
            screenSizeText.text = screenSize.ToString("F1");
        }
        mainComponent.SetActive(false);
    }

    public void UpdateCameraSizeSlider()
    {
        SettingsManager.instance.SetCameraSize(screenSizeSlider.value);
        screenSizeText.text = screenSizeSlider.value.ToString("F1");
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus && mainComponent.activeSelf)
        {
            StartCoroutine(FocusGained());
        }
    }

    IEnumerator FocusGained()
    {
        yield return null;
        firstOption.Select();
    }

    protected override void AfterClosing()
    {
        if (controller)
        {
            controller.hideSettings();
        }
        else
        {
            base.AfterClosing();
        }
    }

    protected override void AfterOpening()
    {
        if (!controller)
        {
            base.AfterOpening();
        }
        firstOption.Select();
    }
}
