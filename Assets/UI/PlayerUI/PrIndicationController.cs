using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrIndicationController : MonoBehaviour
{
    //Players color inventory.
    private ColorInventory colorInventory;

    //different components of the UI for the power indicator. 
    [SerializeField] TMP_Text powerText;
    [SerializeField] Image icon;
    [SerializeField] Image border;
    [SerializeField] Image background;

    private void OnEnable() {
        colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        UpdatePrIndicator();
        colorInventory.onSlotChanged += OnSlotChanged;
        colorInventory.onColorUpdated += UpdatePrIndicator;
        colorInventory.onColorSpellChanged += OnSlotChanged;
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
            powerText.text = buff + "%";
            icon.sprite = currentColor.colorIcon;
            border.color = new Color32(255, 255, 255, 255);
            powerText.gameObject.SetActive(true);
            
        }
        #nullable disable
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
