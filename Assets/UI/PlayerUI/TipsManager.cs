using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using System.Linq;
public class TipsManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] SerializedDictionary<string, Tips> tipsDictionary;
    private SerializedDictionary<string, Tips> currentTipsDictionary;

    [SerializeField] InputActionReference removeTooltipButton;

    private GameObject gameTipsObj;
    private TMP_Text gameTipsText;
    private UIController uIController;
    Action<InputAction.CallbackContext> removeTooltip;

    private void OnEnable() 
    {
        removeTooltip = (InputAction.CallbackContext ctx) => {CloseTips();};
        ResetTipsDictionary();
        removeTooltipButton.action.performed += removeTooltip;
    }

    void OnDisable()
    {
        removeTooltipButton.action.performed -= removeTooltip;
    }

    public void SetUi()
    {
        uIController = FindObjectOfType<UIController>();
        gameTipsObj = uIController.tipsPanel;
        if(!gameTipsObj) Debug.LogError("Did not find tips panel");
        gameTipsText = gameTipsObj.GetComponentInChildren<TMP_Text>();
    }

    public void CloseTips()
    {
        gameTipsObj?.SetActive(false);
        uIController.lightbox.SetActive(false);
        GameManager.instance.Resume();
    }

    public void DisplayTips(string tips)
    {
        if(currentTipsDictionary.ContainsKey(tips)) 
        {
            Tips tipsObj = currentTipsDictionary[tips];
            if(tipsObj.hasBeenDisplayed) return;

            tipsObj.callsNeeded --;
            if(tipsObj.callsNeeded <= 0)
            {
                gameTipsText.text = tipsObj.text;
                tipsObj.hasBeenDisplayed = true;
                gameTipsObj.SetActive(true);
                uIController.lightbox.SetActive(true);
                GameManager.instance.Pause();
            }
        }
        else 
        {
            gameTipsText.text = tips;
            currentTipsDictionary.Add(tips, new Tips(true, tips, 0));
            uIController.lightbox.SetActive(true);
            gameTipsObj.SetActive(true);
            GameManager.instance.Pause();
        }
    }
    private void ResetTipsDictionary()
    {
        currentTipsDictionary = new SerializedDictionary<string, Tips>();
        foreach (KeyValuePair<string, Tips> item in tipsDictionary)
        {
            currentTipsDictionary.Add(item.Key, item.Value.CopyData());
        }
    }
    

    public void LoadData(GameData data)
    {
        ResetTipsDictionary();
        
        foreach (KeyValuePair<string, Tips> item in data.tipsDictionary)
        {
            if(currentTipsDictionary.ContainsKey(item.Key))
            {
                Tips tips = currentTipsDictionary[item.Key];
                tips.callsNeeded = item.Value.callsNeeded;
                tips.hasBeenDisplayed = item.Value.hasBeenDisplayed;
            } else
            {
                currentTipsDictionary.Add(item.Key, item.Value);
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.tipsDictionary = new SerializedDictionary<string, Tips>();
        data.tipsDictionary.AddRange(this.currentTipsDictionary.ToList());
    }
}

[Serializable]
public class Tips
{
    public Tips (bool hasBeenDisplayed, string text, int callsNeeded)
    {
        this.hasBeenDisplayed = hasBeenDisplayed;
        this.text = text;
        this.callsNeeded = callsNeeded;
    }

    public Tips CopyData()
    {
        return new Tips(hasBeenDisplayed, text, callsNeeded);
    }
    [HideInInspector] public bool hasBeenDisplayed;
    [SerializeField] public int callsNeeded;
    [SerializeField, TextArea(5,20)] public string text;
}
