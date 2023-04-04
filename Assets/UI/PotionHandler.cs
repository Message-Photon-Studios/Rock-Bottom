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


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable() {
        colorSlots = colorInventory.colorSlots;
        activeSlot = colorInventory.activeSlot;

        SlotAmountUpdated();

        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSlotsChanged += SlotAmountUpdated;

        slotPositions.Add(new Vector2(0, 0));
        slotPositions.Add(new Vector2(-225, 65));
        slotPositions.Add(new Vector2(-165, 180));
        slotPositions.Add(new Vector2(0, 200));
        slotPositions.Add(new Vector2(165, 180));
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
        if(colorSlots.Count == 3) {
            slots.Add(slot0); colorSlots[0].Init(slot0.GetComponent<Image>());
            slots.Add(slot2); colorSlots[1].Init(slot2.GetComponent<Image>());
            slots.Add(slot4); colorSlots[2].Init(slot4.GetComponent<Image>());

            
        }

        if(colorSlots.Count == 4) {
            slots.Add(slot0); colorSlots[0].Init(slot0.GetComponent<Image>());
            slots.Add(slot1); colorSlots[1].Init(slot1.GetComponent<Image>());
            slots.Add(slot3); colorSlots[2].Init(slot3.GetComponent<Image>());
            slots.Add(slot5); colorSlots[3].Init(slot5.GetComponent<Image>());
        }

        if(colorSlots.Count == 5) {
            slots.Add(slot0); colorSlots[0].Init(slot0.GetComponent<Image>());
            slots.Add(slot1); colorSlots[1].Init(slot1.GetComponent<Image>());
            slots.Add(slot2); colorSlots[2].Init(slot2.GetComponent<Image>());
            slots.Add(slot4); colorSlots[3].Init(slot4.GetComponent<Image>());
            slots.Add(slot5); colorSlots[4].Init(slot5.GetComponent<Image>());
        }

        if(colorSlots.Count == 6) {
            slots.Add(slot0); colorSlots[0].Init(slot0.GetComponent<Image>());
            slots.Add(slot1); colorSlots[1].Init(slot1.GetComponent<Image>());
            slots.Add(slot2); colorSlots[2].Init(slot2.GetComponent<Image>());
            slots.Add(slot3); colorSlots[3].Init(slot3.GetComponent<Image>());
            slots.Add(slot4); colorSlots[4].Init(slot4.GetComponent<Image>());
            slots.Add(slot5); colorSlots[5].Init(slot5.GetComponent<Image>());
        }
    }
}

