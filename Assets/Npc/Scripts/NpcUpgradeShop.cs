using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public abstract class NpcUpgradeShop : MonoBehaviour, IDataPersistence
{
    [SerializeField] int maxBuys;
    int buys;
    [SerializeField] TMP_Text buysText;
    [SerializeField] bool inactiveWhenMaxed;
    [SerializeField] bool deactivatedWhenMaxed;
    [SerializeField] int cost; 
    [SerializeField] int costIncrease;
    [SerializeField] TMP_Text costText;
    [SerializeField] GameObject canvas;
    [SerializeField] private InputActionReference interact;



    void OnEnable()
    {
        interact.action.performed += ShopInteraction;
        if(deactivatedWhenMaxed && buys >= maxBuys) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        interact.action.performed -= ShopInteraction;
        canvas.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OpenShop();
    }

    void OpenShop()
    {
        if(inactiveWhenMaxed && buys >= maxBuys)
        {   
            canvas.SetActive(false);
            return;
        }
        canvas.SetActive(true);
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

    void OnTriggerExit2D(Collider2D other) 
    {
        canvas.SetActive(false);
    }

    private void ShopInteraction(InputAction.CallbackContext ctx)
    {
        if(!canvas.activeSelf) return;
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
