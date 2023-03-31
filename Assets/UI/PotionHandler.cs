using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionHandler : MonoBehaviour
{

    [SerializeField] ColorInventory colorInventory;
    List<ColorSlot> colorSlots;
    int activeSlot;
    [SerializeField] Image slot1;
    [SerializeField] Image slot2;
    [SerializeField] Image slot3;
    [SerializeField] Image slot4;
    [SerializeField] Image slot5;
    [SerializeField] Image slot6;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable() {
        colorSlots = colorInventory.colorSlots;
        activeSlot = colorInventory.activeSlot;

        FillSlots(activeSlot);

        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSlotsChanged += SlotAmountUpdated;

    }

    private void OnDisable() {
        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged -= ActiveColorChanged;
        colorInventory.onColorSlotsChanged -= SlotAmountUpdated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/// <summary>
/// Fills the various colorSlots in the ui with images of current 
/// </summary>
/// <param name="active"></param>
    private void FillSlots(int active) {

        
        slot1 = null;
        slot2 = null;
        slot3 = null;
        slot4 = null;
        slot5 = null;
        slot6 = null;
        

        slot1 = colorSlots[active].image;

        if(colorSlots.Count == 4) {
            //left of active
            slot2 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            //right of active
            slot6 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
            //furthest back
            slot4 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;
        }
        if(colorSlots.Count == 5) {
            //left of active
            slot2 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            slot3 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;

            //right of active
            slot5 = colorSlots[(colorSlots.Count+active-2)%colorSlots.Count].image;
            slot6 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
        }
        if(colorSlots.Count == 6) {
            //left of active
            slot2 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            slot3 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;
            //furthest back
            slot4 = colorSlots[(colorSlots.Count+active+3)%colorSlots.Count].image;
            //right of active
            slot5 = colorSlots[(colorSlots.Count+active-2)%colorSlots.Count].image;
            slot6 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
        }
    }

    private void ColorUpdate() {

    }

    private void ActiveColorChanged(int newSlot) {
        activeSlot = newSlot;
        FillSlots(newSlot);
    }

    private void SlotAmountUpdated() {

    }
}

