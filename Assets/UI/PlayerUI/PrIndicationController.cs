using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrIndicationController : MonoBehaviour
{
    private ColorInventory colorInventory;

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

    private void UpdatePrIndicator(){
        #nullable enable
        GameColor? currentColor = colorInventory.CheckActveColor();
        if(currentColor == null) {
            powerText.gameObject.SetActive(false);
            border.color = new Color32(255, 255, 255, 150);
            background.color = new Color32(75, 59, 33, 150);
            
        } else {
            float buff = 100*(colorInventory.GetColorBuff() + colorInventory.GetActiveColorSpell().powerScale);
            powerText.text = buff + "%";
            icon.sprite = currentColor.colorIcon;
            border.color = new Color32(255, 255, 255, 255);
            background.color = new Color32(75, 59, 33, 200);
            powerText.gameObject.SetActive(true);
            
        }
        #nullable disable
    }

    private void OnSlotChanged(int i) {
        UpdatePrIndicator();
    }
}
