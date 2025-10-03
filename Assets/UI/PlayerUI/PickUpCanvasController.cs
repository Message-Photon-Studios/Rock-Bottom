using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class PickUpCanvasController : UIMenu
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text shortDescText;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text collectText;
    [SerializeField] GameObject interactButtonPrompt;
    [SerializeField] LocalizedString costLocalString;
    [SerializeField] LocalizedString buyLocalString;
    [SerializeField] LocalizedString collectLocalString;
    [SerializeField] LocalizedString colorRetLocalString;
    [SerializeField] LocalizedString closeUiString;
    [SerializeField] LocalizedString nextTextString;
    [SerializeField] Color cantBuyColor;
    Color normalBuyColor;

    void Start()
    {
        if (mainComponent)
        {
            CloseMenu();
        }
        normalBuyColor = costText.color;
    }

    public void SetHealthShrine(HealingShrine healingShrine)
    {
        int cost = healingShrine.CalculatePrice();
        bool hasMoney = Player.instance.playerInventory.HasEnoughCoins(cost);
        SetBuy(healingShrine.GetName(), healingShrine.GetDescription(), cost, hasMoney);
    }

    public void UpdateHealthShrine(HealingShrine healingShrine)
    {
        int cost = healingShrine.CalculatePrice();
        bool hasMoney = Player.instance.playerInventory.HasEnoughCoins(cost);
        UpdateBuy(healingShrine.GetName(), healingShrine.GetDescription(), cost, hasMoney);
    }

    public void SetColorShrine(ColorWell colorWell)
    {
        if (colorWell.GetColorAmount() > 0)
        {
            SetCollect(colorWell.color.name, colorWell.color.description);
            return;
        }

        SetReturnColor(colorWell.color.name, colorWell.color.description);
    }

    public void SetItem(ItemPickup itemPickup)
    {
        if (itemPickup.GetNeedsPayment())
        {
            bool canBuy = Player.instance.playerInventory.HasEnoughCoins(itemPickup.GetItemCost());
            SetBuy(itemPickup.GetItem().GetName(), itemPickup.GetItem().GetDesc(), itemPickup.GetItemCost(), canBuy);
            return;
        }

        SetCollect(itemPickup.GetItem().GetName(), itemPickup.GetItem().GetDesc());
    }

    public void SetBottle(SpellPickup spellPickup)
    {
        if (spellPickup.GetNeedsPayement())
        {
            bool canBuy = Player.instance.playerInventory.HasEnoughCoins(spellPickup.GetSpell().spellCost);
            SetBuy(spellPickup.GetSpell().GetName(), spellPickup.GetSpell().GetDesc(), spellPickup.GetSpell().spellCost, canBuy);
            return;
        }

        SetCollect(spellPickup.GetSpell().GetName(), spellPickup.GetSpell().GetDesc());
    }

    public void SetCrate(string name, string desc, int cost)
    {
        bool canBuy = Player.instance.playerInventory.HasEnoughCoins(cost);
        SetBuy(name, desc, cost, canBuy);
    }

    private void SetReturnColor(string objName, string objDesc)
    {
        costText.gameObject.SetActive(false);
        string returnColorString = colorRetLocalString.GetLocalizedString();
        SetTexts(objName, objDesc, "", returnColorString);
    }

    public void SetCollect(string objName, string objDesc)
    {
        costText.gameObject.SetActive(false);
        string collectString = collectLocalString.GetLocalizedString();
        SetTexts(objName, objDesc, "", collectString);
    }

    public void SetBuy(string objName, string objDesc, int cost, bool canBuy)
    {
        string costString = costLocalString.GetLocalizedString() + " " + cost;
        string buyString = buyLocalString.GetLocalizedString();
        costText.color = canBuy ? normalBuyColor : cantBuyColor;
        costText.gameObject.SetActive(true);
        SetTexts(objName, objDesc, costString, buyString);
        collectText.gameObject.SetActive(canBuy);
        interactButtonPrompt.SetActive(canBuy);
    }

    private void UpdateBuy(string objName, string objDesc, int cost, bool canBuy)
    {
        string costString = costLocalString.GetLocalizedString() + " " + cost;
        string buyString = buyLocalString.GetLocalizedString();
        costText.color = canBuy ? normalBuyColor : cantBuyColor;
        costText.gameObject.SetActive(true);
        UpdateTexts(objName, objDesc, costString, buyString);
        collectText.gameObject.SetActive(canBuy);
        interactButtonPrompt.SetActive(canBuy);
    }

    bool isLast = false;
    public void SetDisplayText(string objName, string objDesc, bool lastText)
    {
        costText.gameObject.SetActive(false);
        string closeString = lastText ? closeUiString.GetLocalizedString() : nextTextString.GetLocalizedString();
        if (!mainComponent.activeSelf || isLast) SetTexts(objName, objDesc, "", closeString);
        else NextText(objDesc, closeString);
        isLast = lastText;
    }

    private void NextText(string newDesc, string closeString)
    {
        collectText.text = closeString;
        shortDescText.text = newDesc;
        collectText.gameObject.SetActive(true);
        interactButtonPrompt.gameObject.SetActive(true);
    }
    private void SetTexts(string objName, string objDesc, string cost, string collect)
    {
        nameText.text = objName;
        shortDescText.text = objDesc;
        costText.text = cost;
        collectText.text = collect;
        collectText.gameObject.SetActive(true);
        interactButtonPrompt.gameObject.SetActive(true);

        OpenMenu();
    }

    private void UpdateTexts(string objName, string objDesc, string cost, string collect)
    {
        nameText.text = objName;
        shortDescText.text = objDesc;
        costText.text = cost;
        collectText.text = collect;
    }
}
