using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class TizoShopInteractable : InteractionObject
{

    [SerializeField] LocalizedString title;
    [SerializeField] LocalizedString desc;
    [SerializeField] PickUpCanvasController pickUpCanvas;
    [SerializeField] TizoShop tizoShop;
    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);

        /*
        if(isClose) pickUpCanvas.SetCollect(title.GetLocalizedString(), desc.GetLocalizedString());
        else pickUpCanvas.CloseUi();
        */

        if(!isClose) tizoShop.ShopInteract(false);
    }

    protected override void PlayerInteract()
    {
        tizoShop.ShopInteract();
    }
}
