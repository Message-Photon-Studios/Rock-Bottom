using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private String[] soundGroups;

    public const string screenSizeKey = "screenSize";
    private int selectedPixelSize = 100;

    void Awake()
    {
        if (instance == null) instance = this;
        else this.enabled = false;
    }
    private void Start()
    {
        foreach (string group in soundGroups)
        {
            if (PlayerPrefs.HasKey(group))
            {
                LoadVolume(group);
            }
            else
            {
                SetVolume(1, group);
            }
        }

        LoadCameraSize();
    }

    public void SetVolume(float volume, string group)
    {
        mainAudioMixer.SetFloat(group, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(group, volume);
    }

    private void LoadVolume(string group)
    {
        float volume = PlayerPrefs.GetFloat(group);
        SetVolume(volume, group);
    }

    public void SetCameraSize(float cameraSize)
    {
        if(SceneManager.GetActiveScene().name != "MainMenu") Camera.main.orthographicSize = cameraSize;
        PlayerPrefs.SetFloat(screenSizeKey, cameraSize);
    }

    public void LoadCameraSize()
    {
        if(PlayerPrefs.HasKey(screenSizeKey) && SceneManager.GetActiveScene().name != "MainMenu")
            Camera.main.orthographicSize = PlayerPrefs.GetFloat(screenSizeKey);
    }
}

