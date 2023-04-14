using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for handling all the UI features of the ColorSlotUI.
/// </summary>
public class ColorSlotController : MonoBehaviour
{
    //The players ColorInventory, used to access UnityActions and information.
    [SerializeField] ColorInventory colorInventory;
    //Color slots the player currently has.
    List<ColorSlot> colorSlots;
    //List that holds all the UI elements for the current slots.
    List<RectTransform> slots;
    //All created UI elements for slots.
    [SerializeField] RectTransform slot0;
    [SerializeField] RectTransform slot1;
    [SerializeField] RectTransform slot2;
    [SerializeField] RectTransform slot3;
    [SerializeField] RectTransform slot4;
    [SerializeField] RectTransform slot5;
    //Positions of all UI elements for slots.
    List<Vector2> slotPositions = new List<Vector2>();

    //Different sets of slots used for different amount of color slots the player has.
    List<RectTransform> setOf3;
    List<RectTransform> setOf4;
    List<RectTransform> setOf5;
    List<RectTransform> setOf6;

    # region Setup
    /// <summary>
    /// When enabling the Player In game UI, set up the script.
    /// </summary>
    private void OnEnable() {
        //Fetch the players current colors.
        colorSlots = colorInventory.colorSlots;

        //Set up the different sets.
        setOf3 = new List<RectTransform>{slot0, slot2, slot4};
        setOf4 = new List<RectTransform>{slot0, slot1, slot3, slot5};
        setOf5 = new List<RectTransform>{slot0, slot1, slot2, slot4, slot5};
        setOf6 = new List<RectTransform>{slot0, slot1, slot2, slot3, slot4, slot5};

        //Attach local functions to UnityActions.
        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSlotsChanged += SlotAmountUpdated;

        //Get positions of the containers for the UI slots.
        slotPositions.Add(((RectTransform)slot0.parent).anchoredPosition);
        slotPositions.Add(((RectTransform)slot1.parent).anchoredPosition);
        slotPositions.Add(((RectTransform)slot2.parent).anchoredPosition);
        slotPositions.Add(((RectTransform)slot3.parent).anchoredPosition);
        slotPositions.Add(((RectTransform)slot4.parent).anchoredPosition);
        slotPositions.Add(((RectTransform)slot5.parent).anchoredPosition);

        SlotAmountUpdated();
        RotateSlots(1);
        RotateSlots(-1);
    }

    //When turning off UI, detatch UnityActions from local functions. 
    private void OnDisable() {
        colorInventory.onColorUpdated -= ColorUpdate;
        colorInventory.onSlotChanged -= ActiveColorChanged;
        colorInventory.onColorSlotsChanged -= SlotAmountUpdated;
    }
    #endregion
    #region SlotMovement
    /// <summary>
    /// Given a slot and where it currently is, give it a new position and order in 
    /// hierarchy according to the direction it's moving in.
    /// </summary>
    /// <param name="rect"></param> UI slot.
    /// <param name="current"></param> Integer for which slot in slots[] is being moved, 0-5.
    /// <param name="dir"></param> Integer for which direction it's moving in, 1=clockwise -1=CounterClockwise.
    private void MoveSlotTo(RectTransform rect, int current, int dir) {

        if(current == 0 && dir == 1) { // nr0 going counter clockwise
            if(slots.Count == 3) {MovingSlot(rect, 4, 2);}
            else {MovingSlot(rect, 5, 4);}
        } else if( current == 0) { //nr0 going clockwise
            if(slots.Count == 3) {MovingSlot(rect, 2, 1);}
            else {MovingSlot(rect, 1, 3);}
        }
        
         else if(current == 1 && dir == 1){  //nr1 going counter clockwise
            MovingSlot(rect, 0, 5);
        } else if(current == 1) { //nr1 going clockwise
            if(slots.Count == 3) {MovingSlot(rect, 4, 2);}
            if(slots.Count == 4) {MovingSlot(rect, 3, 0);}
            if(slots.Count == 5 || slots.Count == 6) {MovingSlot(rect, 2, 1);}
        }
        
         else if(current == 2 && dir == 1) { //nr2 going counter clockwise
            if(slots.Count == 3) {MovingSlot(rect, 2, 1);}
            else {MovingSlot(rect, 1, 3);}
        } else if(current == 2) { //nr2 going clockwise
            if(slots.Count == 3) {MovingSlot(rect, 0, 5);}
            if(slots.Count == 4) {MovingSlot(rect, 5, 4);}
            if(slots.Count == 5) {MovingSlot(rect, 4, 2);}
            if(slots.Count == 6) {MovingSlot(rect,3, 0);}
        }
        
         else if(current == 3 && dir == 1) { //nr3 going counter clockwise
            if(slots.Count == 4) {MovingSlot(rect, 3, 0);}
            if(slots.Count == 5 || slots.Count == 6) {MovingSlot(rect, 2, 1);}
        } else if(current == 3) { //nr3 going clockwise
            if(slots.Count == 4) {MovingSlot(rect, 0, 5);}
            if(slots.Count == 5) {MovingSlot(rect, 5, 4);}
            if(slots.Count == 6) {MovingSlot(rect, 4, 2);}
        } 

        else if(current == 4 && dir == 1) { //nr4 going counter clockwise
            if(slots.Count == 5) {MovingSlot(rect, 4, 2);}
            if(slots.Count == 6) {MovingSlot(rect, 3, 0);}
        } else if(current == 4) { //nr4 going clockwise
            if(slots.Count == 5) {MovingSlot(rect, 0, 5);}
            if(slots.Count == 6) {MovingSlot(rect, 5, 4);}
        }

        else if(current == 5 && dir == 1) { //nr5 going counter clockwise
            MovingSlot(rect, 4, 2);
        } else if(current == 5) { //nr5 going clockwise
            MovingSlot(rect, 0, 5);
        }
    }

    private void MovingSlot(RectTransform rect, int pos, int layer) {
        rect.anchoredPosition = slotPositions[pos]; 
        rect.SetSiblingIndex(layer); 

        float scale = 1f;
        if(pos == 5 || pos == 1) scale = 0.7f;
        if(pos == 4 || pos == 2) scale = 0.6f;
        if(pos == 3) scale = 0.6f;
        rect.transform.localScale = new Vector3(scale, scale, scale);
    }

/// <summary>
/// Moves the different containers for slots around when cycling colors according to direction. 
/// Q=CounterClockwise=-1, E=Clockwise=1.
/// Also keeps the Slots[] sorted so that the active color is slots[0].
/// </summary>
/// <param name="dir"></param> Which direction the slots are rotating in.
    private void RotateSlots(int dir) {

        for(int i = 0; i < slots.Count; i++) {
            MoveSlotTo((RectTransform)slots[i].parent, i, dir);
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
    #endregion
    #region UnityActions
    //When a color is updated, call this. Currently not used.
    private void ColorUpdate() {
    }

    /// <summary>
    /// When active color has changed, rotate UI according to direction.
    /// </summary>
    /// <param name="dir"></param> Direction to rotate in.
    private void ActiveColorChanged(int dir) {
        RotateSlots(dir);
    }

    /// <summary>
    /// When amount of color slots is updated, change UI components to new amount accordingly.
    /// This deactivates containers for slots that currently aren't used and initializes images for used slots.
    /// </summary>
    private void SlotAmountUpdated() {
        colorSlots = colorInventory.colorSlots;
        slots = new List<RectTransform>();

        

        foreach(RectTransform slot in setOf6) {
            slot.parent.gameObject.SetActive(false);
        }

        if(colorSlots.Count == 3) {
            for(int i = 0; i<3; i++) {
                slots.Add(setOf3[i]);
                colorSlots[i].Init(setOf3[i].GetComponent<Image>());
                setOf3[i].parent.gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 4) {
            for(int i = 0; i<4; i++) {
                slots.Add(setOf4[i]);
                colorSlots[i].Init(setOf4[i].GetComponent<Image>());
                setOf4[i].parent.gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 5) {
            for(int i = 0; i<5; i++) {
                slots.Add(setOf5[i]);
                colorSlots[i].Init(setOf5[i].GetComponent<Image>());
                setOf5[i].parent.gameObject.SetActive(true);
            }
        }

        if(colorSlots.Count == 6) {
            for(int i = 0; i<6; i++) {
                slots.Add(setOf6[i]);
                colorSlots[i].Init(setOf6[i].GetComponent<Image>());
                setOf6[i].parent.gameObject.SetActive(true);
            }
        }
    }
    #endregion
}