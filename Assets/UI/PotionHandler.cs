using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionHandler : MonoBehaviour
{

    [SerializeField] ColorInventory colorInventory;
    List<ColorSlot> colorSlots;
    int activeSlot;

    [SerializeField] List<RectTransform> slots;
    List<Vector2> slotPositions = new List<Vector2>();


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable() {
        colorSlots = colorInventory.colorSlots;
        activeSlot = colorInventory.activeSlot;

        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSlotsChanged += SlotAmountUpdated;

        slotPositions.Add(new Vector2(0, 0));
        slotPositions.Add(new Vector2(-225, 65));
        slotPositions.Add(new Vector2(-165, 180));
        slotPositions.Add(new Vector2(0, 200));
        slotPositions.Add(new Vector2(-165, 180));
        slotPositions.Add(new Vector2(225, 65));

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

    private void MoveSlotTo(RectTransform rect, int current, int dir) {
        
        if(current == 0 && dir == 1) {
            rect.anchoredPosition = slotPositions[1];
        } else if( current == 0) {
            rect.anchoredPosition = slotPositions[5];
        } else if(current == 1 && dir == 1){
            rect.anchoredPosition = slotPositions[3];
        } else if(current == 1) {
            rect.anchoredPosition = slotPositions[0];
        } else if(current == 2 && dir == 1) {
            rect.anchoredPosition = slotPositions[5];
        } else if(current == 2) {
            rect.anchoredPosition = slotPositions[1];
        } else if(current == 3 && dir == 1) {
            rect.anchoredPosition = slotPositions[0];
        } else if(current == 3) {
            rect.anchoredPosition = slotPositions[3];
        } 
    }

/// <summary>
/// Moves the different slots around when cycling colors. Q = left = -1, E = right = 1
/// </summary>
/// <param name="active"></param>
    private void RotateSlots(int active, int dir) {
//slot0Pos = (colorSlots.Count+slot0Pos)%colorSlots.Count;

        for(int i = 0; i < slots.Count; i++) {
            MoveSlotTo(slots[i], i, dir);
        }

        if(dir == 1) {
            RectTransform temp = slots[0];
            slots.Remove(slots[0]);
            slots.Insert(slots.Count, temp);
        } else if(dir == -1) {
            RectTransform temp = slots[slots.Count-1];
            slots.Remove(slots[slots.Count-1]);
            slots.Insert(0, temp);
        }
    }

    private void ColorUpdate() {

    }

    private void ActiveColorChanged(int newSlot, int dir) {
        activeSlot = newSlot;
        Debug.Log(dir);
        RotateSlots(newSlot, dir);
    }

    private void SlotAmountUpdated() {

    }
}

