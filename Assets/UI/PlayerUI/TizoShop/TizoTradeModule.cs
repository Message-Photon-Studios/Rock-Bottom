using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TizoTradeModule : MonoBehaviour
{
    [SerializeField] Image[] costImages;
    [SerializeField] Image[] getImages;
 
    int tradeIndex;
    TizoTrade tizoTrade;
    TizoShop tizoShop;
    bool hasBought = false;
    public void TryTrade()
    {
        if(hasBought) return;
        if(tizoShop.TryTrade(tradeIndex)) 
        {
            hasBought = true;
            gameObject.SetActive(false);
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
}
