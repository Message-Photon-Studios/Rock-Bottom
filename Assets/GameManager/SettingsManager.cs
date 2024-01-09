using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{

    private int selectedPixelSize = 100;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

