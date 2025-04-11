using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private string group;
    [SerializeField] private TMP_Text number;
    private SettingsManager settingsManager;

    private void OnEnable() {
        settingsManager = FindObjectOfType<SettingsManager>();
        LoadSlider();
    }

    public void UpdateSlider() {
        float volume = slider.value;
        settingsManager.SetVolume(volume, group);
        number.text = Mathf.RoundToInt(slider.value*100).ToString();
    }

    private void LoadSlider() {
        slider.value = PlayerPrefs.GetFloat(group);
        number.text = Mathf.RoundToInt(slider.value*100).ToString();
    }
    
}
