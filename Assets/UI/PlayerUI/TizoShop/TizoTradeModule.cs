using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TizoTradeModule : MonoBehaviour, ISelectHandler
{
    [SerializeField] Image cantBuyImage;
    [SerializeField] Color shadedColor; 
    [SerializeField] Image[] costImages;
    [SerializeField] Image[] getImages;
 
    int tradeIndex;
    TizoTrade tizoTrade;
    TizoShop tizoShop;
    public bool hasBought {get; private set;} = false;
    public void TryTrade()
    {
        if(hasBought) return;
        if(tizoShop.TryTrade(tradeIndex)) 
        {
            hasBought = true;
            GetComponent<Button>().interactable = false;
        }
    }

    public void SetTradeModule(TizoTrade tizoTrade, int tradeIndex, TizoShop tizoShop)
    {
        this.tizoTrade = tizoTrade;
        this.tradeIndex = tradeIndex;
        this.tizoShop = tizoShop;

        foreach (Image image in costImages)
        {
            image.gameObject.SetActive(false);
        }
        
        foreach(Image image in getImages)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < tizoTrade.costItems.Length && i < costImages.Length; i++)
        {
            costImages[i].sprite = tizoTrade.costItems[i].sprite;
            costImages[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < tizoTrade.getItems.Length && i < getImages.Length; i++)
        {
            getImages[i].sprite = tizoTrade.getItems[i].sprite;
            getImages[i].gameObject.SetActive(true);
        }
    }

    public void UpdateUi()
    {
        bool canbuy = true;
        for (int i = 0; i < tizoTrade.costItems.Length && i < costImages.Length; i++)
        {
            if(Player.instance.playerInventory.HasItemWithName(tizoTrade.costItems[i].name))
            {
                costImages[i].color = Color.white;
            } else
            {
                costImages[i].color = shadedColor;
                canbuy = false;
            }
        }

        cantBuyImage.gameObject.SetActive(!canbuy || hasBought);
    }

    public void OnSelect(BaseEventData eventData)
    {
        tizoShop.UpdateInfoCards(tizoTrade);
    }
}
