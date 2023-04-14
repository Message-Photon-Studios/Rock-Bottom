using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private ColorInventory colorInventory;
    [SerializeField] List<GameObject> colorSlotContainers;

    private void OnEnable() {
        colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        colorInventory.onColorSlotsChanged += colorSlotUpdate;
        colorSlotUpdate();

    }

    private void OnDisable() {
        colorInventory.onColorSlotsChanged -= colorSlotUpdate;
    }


    private void colorSlotUpdate() {
        foreach(GameObject colorSlotContainer in colorSlotContainers) {
            colorSlotContainer.SetActive(false);
        }

        if(colorInventory.colorSlots.Count == 3) {colorSlotContainers[0].SetActive(true);}
        if(colorInventory.colorSlots.Count == 4) {colorSlotContainers[1].SetActive(true);}
        if(colorInventory.colorSlots.Count == 5) {colorSlotContainers[2].SetActive(true);}
        if(colorInventory.colorSlots.Count == 6) {colorSlotContainers[3].SetActive(true);}
    }
}
