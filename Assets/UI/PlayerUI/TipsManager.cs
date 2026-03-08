using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.Localization;
public class TipsManager : MonoBehaviour
{
    [SerializeField] LocalizedString[] loadingTips;


    public string GetLoadingTips()
    {
        return loadingTips[UnityEngine.Random.Range(0, loadingTips.Length)].GetLocalizedString();
    }
}