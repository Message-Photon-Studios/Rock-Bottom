using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionHandler : MonoBehaviour
{

    [SerializeField] ColorInventory colorInventory;
    List<ColorSlot> colorSlots;
    int activeSlot;

    List<RectTransform> slots;
    [SerializeField] RectTransform slot0;
    [SerializeField] RectTransform slot1;
    [SerializeField] RectTransform slot2;
    [SerializeField] RectTransform slot3;
    [SerializeField] RectTransform slot4;
    [SerializeField] RectTransform slot5;
    List<Vector2> slotPositions = new List<Vector2>();

    List<RectTransform> setOf3;
    List<RectTransform> setOf4;
    List<RectTransform> setOf5;
    List<RectTransform> setOf6;

    // Start is called before the first frame update
    void Start()
    {
    }

//TODO Comments on the code
    private void OnEnable() {
        colorSlots = colorInventory.colorSlots;
        activeSlot = colorInventory.activeSlot;

        setOf3 = new List<RectTransform>{slot0, slot2, slot4};
        setOf4 = new List<RectTransform>{slot0, slot1, slot3, slot5};
        setOf5 = new List<RectTransform>{slot0, slot1, slot2, slot4, slot5};
        setOf6 = new List<RectTransform>{slot0, slot1, slot2, slot3, slot4, slot5};

        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSlotsChanged += SlotAmountUpdated;

        slotPositions.Add(slot0.anchoredPosition);
        slotPositions.Add(slot1.anchoredPosition);
        slotPositions.Add(slot2.anchoredPosition);
        slotPositions.Add(slot3.anchoredPosition);
        slotPositions.Add(slot4.anchoredPosition);
        slotPositions.Add(slot5.anchoredPosition);

        SlotAmountUpdated();
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

        if(current == 0 && dir == 1) { // nr0 going counter clockwise
            if(slots.Count == 3) rect.anchoredPosition = slotPositions[4];
            else rect.anchoredPosition = slotPositions[5];
        } else if( current == 0) { //nr0 going clockwise
            if(slots.Count == 3) rect.anchoredPosition = slotPositions[2];
            else rect.anchoredPosition = slotPositions[1];
        }
        
         else if(current == 1 && dir == 1){  //nr1 going counter clockwise
            rect.anchoredPosition = slotPositions[0];
        } else if(current == 1) { //nr1 going clockwise
            if(slots.Count == 3) rect.anchoredPosition = slotPositions[4];
            if(slots.Count == 4) rect.anchoredPosition = slotPositions[3];
            if(slots.Count == 5 || slots.Count == 6) rect.anchoredPosition = slotPositions[2];
        }
        
         else if(current == 2 && dir == 1) { //nr2 going counter clockwise
            if(slots.Count == 3) rect.anchoredPosition = slotPositions[2];
            else rect.anchoredPosition = slotPositions[1];
        } else if(current == 2) { //nr2 going clockwise
            if(slots.Count == 3) rect.anchoredPosition = slotPositions[0];
            if(slots.Count == 4) rect.anchoredPosition = slotPositions[5];
            if(slots.Count == 5) rect.anchoredPosition = slotPositions[4];
            if(slots.Count == 6) rect.anchoredPosition = slotPositions[3];
        }
        
         else if(current == 3 && dir == 1) { //nr3 going counter clockwise
            if(slots.Count == 4) rect.anchoredPosition = slotPositions[3];
            if(slots.Count == 5 || slots.Count == 6) rect.anchoredPosition = slotPositions[2];
        } else if(current == 3) { //nr3 going clockwise
            if(slots.Count == 4) rect.anchoredPosition = slotPositions[0];
            if(slots.Count == 5) rect.anchoredPosition = slotPositions[5];
            if(slots.Count == 6) rect.anchoredPosition = slotPositions[4];
        } 

        else if(current == 4 && dir == 1) { //nr4 going counter clockwise
            if(slots.Count == 5) rect.anchoredPosition = slotPositions[4];
            if(slots.Count == 6) rect.anchoredPosition = slotPositions[3];
        } else if(current == 4) { //nr4 going clockwise
            if(slots.Count == 5) rect.anchoredPosition = slotPositions[0];
            if(slots.Count == 6) rect.anchoredPosition = slotPositions[5];
        }

        else if(current == 5 && dir == 1) { //nr5 going counter clockwise
            rect.anchoredPosition = slotPositions[4];
        } else if(current == 5) { //nr5 going clockwise
            rect.anchoredPosition = slotPositions[0];
        }
    }

/// <summary>
/// Moves the different slots around when cycling colors. Q = counter clockwise = -1, E = clockwise = 1
/// </summary>
/// <param name="active"></param>
    private void RotateSlots(int active, int dir) {

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
        RotateSlots(newSlot, dir);
    }

    private void SlotAmountUpdated() {
        colorSlots = colorInventory.colorSlots;
        slots = new List<RectTransform>();

        

        foreach(RectTransform slot in setOf6) {
            slot.gameObject.SetActive(false);
        }

        if(colorSlots.Count == 3) {
            for(int i = 0; i<4; i++) {
                slots.Add(setOf3[i]);
                colorSlots[i].Init(setOf3[i].GetComponent<Image>());
                setOf3[i].gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 4) {
            for(int i = 0; i<5; i++) {
                slots.Add(setOf4[i]);
                colorSlots[i].Init(setOf4[i].GetComponent<Image>());
                setOf4[i].gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 5) {
            for(int i = 0; i<6; i++) {
                slots.Add(setOf5[i]);
                colorSlots[i].Init(setOf5[i].GetComponent<Image>());
                setOf5[i].gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 6) {
            for(int i = 0; i<7; i++) {
                slots.Add(setOf6[i]);
                colorSlots[i].Init(setOf6[i].GetComponent<Image>());
                setOf6[i].gameObject.SetActive(true);
            }
        }
    }
}

