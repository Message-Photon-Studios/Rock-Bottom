using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionHandler : MonoBehaviour
{

    [SerializeField] ColorInventory colorInventory;
    List<ColorSlot> colorSlots;
    int activeSlot;

    [SerializeField] RectTransform slot0;
    int slot0Pos = 0;
    [SerializeField] RectTransform slot1;
    int slot1Pos = 1;
    [SerializeField] RectTransform slot2;
    int slot2Pos = 2;
    [SerializeField] RectTransform slot3;
    int slot3Pos = 3;
    [SerializeField] RectTransform slot4;
    int slot4Pos = 4;
    [SerializeField] RectTransform slot5;
    int slot5Pos = 5;

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

        slotPositions.Add(slot0.anchoredPosition);
        slotPositions.Add(slot1.anchoredPosition);
        //slotPositions.Add(slot2.position);
        slotPositions.Add(slot3.anchoredPosition);
        //slotPositions.Add(slot4.position);
        slotPositions.Add(slot5.anchoredPosition);

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
        
        if(dir == -1) {
            if(colorSlots.Count == 4) {
                if (current == 0) {
                    rect.anchoredPosition = slotPositions[3];
                } else {
                    rect.anchoredPosition = slotPositions[current + dir*-1];
                }

            }
        }
        
        
        //rect.anchoredPosition = slotPositions[slot];

    }

/// <summary>
/// Moves the different slots around when cycling colors. Q = left = -1, E = right = 1
/// </summary>
/// <param name="active"></param>
    private void MoveSlots(int active, int dir) {

        if(colorSlots.Count == 4) {
            MoveSlotTo(slot0, slot0Pos, dir);
            slot0Pos = (colorSlots.Count+slot0Pos)%colorSlots.Count;
            MoveSlotTo(slot1, slot1Pos, dir);
            slot1Pos = (colorSlots.Count+slot1Pos)%colorSlots.Count;
            MoveSlotTo(slot3, slot3Pos, dir);
            slot3Pos = (colorSlots.Count+slot3Pos)%colorSlots.Count;
            MoveSlotTo(slot5, slot5Pos, dir);
            slot5Pos = (colorSlots.Count+slot5Pos)%colorSlots.Count;
        }


        /*
        slot0 = null;
        slot1 = null;
        slot2 = null;
        slot3 = null;
        slot4 = null;
        slot5 = null;
        

        slot0 = colorSlots[active].image;

        if(colorSlots.Count == 4) {
            //left of active
            slot1 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            //right of active
            slot5 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
            //furthest back
            slot3 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;
        }
        if(colorSlots.Count == 5) {
            //left of active
            slot1 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            slot2 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;

            //right of active
            slot4 = colorSlots[(colorSlots.Count+active-2)%colorSlots.Count].image;
            slot5 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
        }
        if(colorSlots.Count == 6) {
            //left of active
            slot1 = colorSlots[(colorSlots.Count+active+1)%colorSlots.Count].image;
            slot2 = colorSlots[(colorSlots.Count+active+2)%colorSlots.Count].image;
            //furthest back
            slot3 = colorSlots[(colorSlots.Count+active+3)%colorSlots.Count].image;
            //right of active
            slot4 = colorSlots[(colorSlots.Count+active-2)%colorSlots.Count].image;
            slot5 = colorSlots[(colorSlots.Count+active-1)%colorSlots.Count].image;
        }
        */
    }

    private void ColorUpdate() {

    }

    private void ActiveColorChanged(int newSlot, int dir) {
        activeSlot = newSlot;
        Debug.Log(dir);
        MoveSlots(newSlot, dir);
    }

    private void SlotAmountUpdated() {

    }
}

