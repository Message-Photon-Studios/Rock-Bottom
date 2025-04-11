using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrIndicationController : MonoBehaviour
{
    //Players color inventory.
    private ColorInventory colorInventory;

    private ItemInventory itemInventory;

    //different components of the UI for the power indicator. 
    [SerializeField] TMP_Text powerText;
    [SerializeField] Image icon;
    [SerializeField] Image border;
    [SerializeField] Image background;

    private void OnEnable() {
        colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        itemInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        UpdatePrIndicator();
        colorInventory.onSlotChanged += OnSlotChanged;
        colorInventory.onColorUpdated += UpdatePrIndicator;
        colorInventory.onColorSpellChanged += OnSlotChanged;
        itemInventory.onItemPickedUpOrRemoved += UpdatePrIndicator;
    }

    private void OnDisable() {
        colorInventory.onSlotChanged -= OnSlotChanged;
        colorInventory.onColorUpdated -= UpdatePrIndicator;
        colorInventory.onColorSpellChanged -= OnSlotChanged;
        itemInventory.onItemPickedUpOrRemoved -= UpdatePrIndicator;
    }

    /// <summary>
    /// called every time number should update and sets parameters. 
    /// </summary>
    private void UpdatePrIndicator(){
        #nullable enable
        GameColor? currentColor = colorInventory.CheckActveColor();
        if(currentColor == null) {
            powerText.gameObject.SetActive(false);
            border.color = new Color32(255, 255, 255, 150);
            
        } else {
            float buff = 100*(colorInventory.GetColorBuff() + colorInventory.GetActiveColorSpell().powerScale);
            buff = Mathf.Round(buff);
            if (buff <= 10) buff = 10;
            powerText.text = (int) buff + "%";
            icon.sprite = currentColor.colorIcon;
            border.color = new Color32(255, 255, 255, 255);
            powerText.gameObject.SetActive(true);
            
        }
        #nullable disable
    }

    private void UpdatePrIndicator(Item item) {
        UpdatePrIndicator();
    }

    /// <summary>
    /// Catching unity actions that send an int with it and calls
    /// UpdatePrIndicator without it as it's unessesary for this.
    /// </summary>
    /// <param name="i"></param>
    private void OnSlotChanged(int i) {
        UpdatePrIndicator();
    }
}
