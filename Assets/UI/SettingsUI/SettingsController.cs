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

    public Resolution[] resolutions;
    [SerializeField] TMP_Dropdown resolutionDropdown;

    void Start()
    {
        if (PlayerPrefs.HasKey(SettingsManager.screenSizeKey))
        {
            float screenSize = PlayerPrefs.GetFloat(SettingsManager.screenSizeKey);
            screenSizeSlider.value = screenSize;
            screenSizeText.text = screenSize.ToString("F1");
        }
        mainComponent.SetActive(false);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
               resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
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

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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
