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
public class TipsManager : UIMenu, IDataPersistence
{
    [SerializeField] SerializedDictionary<string, Tips> tipsDictionary;
    private SerializedDictionary<string, Tips> currentTipsDictionary;

    [SerializeField] LocalizedString[] loadingTips;

    Action<InputAction.CallbackContext> removeTooltip;

    void Start()
    {
        removeTooltip = (InputAction.CallbackContext ctx) => { CloseTips(); };
        Player.instance.interactAction += BeforeClosing;
        mainComponent = Player.instance.playerUi.tipsPanel;
    }

    void OnDestroy()
    {
        Player.instance.interactAction -= BeforeClosing;
    }

    public void CloseTips()
    {
        if (!Player.instance) return;
        if (!mainComponent.activeSelf) return;
        CloseMenu();
        Player.instance.playerUi.lightbox.SetActive(false);
        GameManager.instance.Resume();
    }

    public void DisplayTips(string tipsKey)
    {
        if(!Player.instance) return;
        if(!GameManager.instance.allowsTips) return;
        if(currentTipsDictionary.ContainsKey(tipsKey)) 
        {
            Tips tipsObj = currentTipsDictionary[tipsKey];
            if(tipsObj.hasBeenDisplayed) return;

            tipsObj.callsNeeded --;

            if(tipsObj.callsNeeded <= 0)
            {
                mainComponent.GetComponentInChildren<TMP_Text>().text = tipsObj.text.GetLocalizedString();
                tipsObj.hasBeenDisplayed = true;
                OpenMenu();
                Player.instance.playerUi.lightbox.SetActive(true);
                GameManager.instance.Pause();
            }
        }
        else 
        {
            mainComponent.GetComponentInChildren<TMP_Text>().text = tipsKey;
            currentTipsDictionary.Add(tipsKey, new Tips(true, tipsDictionary[tipsKey].text, 0));
            Player.instance.playerUi.lightbox.SetActive(true);
            OpenMenu();
            GameManager.instance.Pause();
        }
    }

    protected override void BeforeClosing()
    {
        CloseTips();
    }

    private void ResetTipsDictionary()
    {
        currentTipsDictionary = new SerializedDictionary<string, Tips>();
        foreach (KeyValuePair<string, Tips> item in tipsDictionary)
        {
            currentTipsDictionary.Add(item.Key, item.Value.CopyData());
        }
    }

    public string GetLoadingTips()
    {
        return loadingTips[UnityEngine.Random.Range(0, loadingTips.Length)].GetLocalizedString();
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
    public Tips (bool hasBeenDisplayed, LocalizedString text, int callsNeeded)
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
    [SerializeField] public LocalizedString text;
}
