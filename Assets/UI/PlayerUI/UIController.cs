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

        var initialSlotCount = 3;
        colorSlotContainers[colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
    }
}
