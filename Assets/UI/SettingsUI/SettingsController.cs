using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<Resolution> resolutions = new List<Resolution>();
    [SerializeField] TMP_Dropdown resolutionDropdown;

    void Start()
    {
        if (PlayerPrefs.HasKey(SettingsManager.screenSizeKey))
        {
            float screenSize = PlayerPrefs.GetFloat(SettingsManager.screenSizeKey);
            screenSizeSlider.value = screenSize;
            screenSizeText.text = ((int)screenSizeSlider.value).ToString();
        }
        mainComponent.SetActive(false);

        resolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        int refreshRate = Screen.currentResolution.refreshRate;

        foreach(Resolution resolution in Screen.resolutions)
        {
            if(refreshRate == resolution.refreshRate)
            {
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);
                resolutions.Add(resolution);

                if(resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = resolutions.Count-1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void UpdateCameraSizeSlider()
    {
        SettingsManager.instance.SetCameraSize(screenSizeSlider.value);
        screenSizeText.text = ((int)screenSizeSlider.value).ToString();
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        StartCoroutine(AdjustCameras());
    }

    IEnumerator AdjustCameras()
    {
        yield return null;
        AspectRatioUtility[] cameras = GameObject.FindObjectsOfType<AspectRatioUtility>();
        foreach(AspectRatioUtility camera in cameras)
        {
            camera.Adjust();
        }
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
