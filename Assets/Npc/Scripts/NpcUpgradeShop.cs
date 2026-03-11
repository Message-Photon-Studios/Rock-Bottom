using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public abstract class NpcUpgradeShop : InteractionObject, IDataPersistence
{
    [SerializeField] int maxBuys;
    int buys;
    [SerializeField] TMP_Text buysText;
    [SerializeField] bool inactiveWhenMaxed;
    [SerializeField] bool deactivatedWhenMaxed;
    [SerializeField] int cost; 
    [SerializeField] int costIncrease;
    [SerializeField] TMP_Text costText;
    [SerializeField] UIMenu canvas;



    void OnEnable()
    {
        if(deactivatedWhenMaxed && buys >= maxBuys) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        canvas.CloseMenu();
    }

    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);
        
        if(isClose) OpenShop();
        else canvas.CloseMenu();
    }

    void OpenShop()
    {
        if(inactiveWhenMaxed && buys >= maxBuys)
        {   
            canvas.CloseMenu();
            return;
        }
        canvas.OpenMenu();
        if(GameManager.instance.GetPetrifiedPigmentAmount() < cost+buys*costIncrease)
        {
            costText.color = Color.red;
        } else
        {
            costText.color = Color.white;
        }

        costText.text = cost.ToString();

        if(buys >= maxBuys)
        {
            buysText.color = Color.red;
        } else
        {
            buysText.color = Color.white;
        }

        buysText.text = buys + "/" + maxBuys;
    }

    protected override void PlayerInteract()
    {
        if(!canvas.mainComponent.activeSelf) return;
        if(buys >= maxBuys) return;
        if(GameManager.instance.TryRemovePetrifiedPigment(cost+buys*costIncrease))
        {
            buys ++;
            Shop();
            OpenShop();

            DataPersistenceManager.instance.SaveGame();
            if(deactivatedWhenMaxed && buys >= maxBuys) gameObject.SetActive(false);
        }
        
    }

    protected abstract void Shop();

    void IDataPersistence.LoadData(GameData data)
    {
        if(this == null) return;
        if(data.permanentShopBuys != null && data.permanentShopBuys.ContainsKey(name))
        {
            buys = data.permanentShopBuys[name];
        } else
        {
            buys = 0;
        }
        if(deactivatedWhenMaxed && buys >= maxBuys) gameObject.SetActive(false);
    }

    void IDataPersistence.SaveData(GameData data)
    {
        if(this == null) return;
        if(data.permanentShopBuys == null) data.permanentShopBuys = new AYellowpaper.SerializedCollections.SerializedDictionary<string, int>();
        
        if(data.permanentShopBuys.ContainsKey(name))
        {
            data.permanentShopBuys[name] = buys;
        } else
        {
            data.permanentShopBuys.Add(name, buys);
        }
    }
}
