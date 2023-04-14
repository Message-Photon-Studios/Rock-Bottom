using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for handling all the UI features of the ColorSlotUI.
/// </summary>
public class ColorSlotController : MonoBehaviour
{
    ColorInventory colorInventory;
    //Color slots the player currently has.
    List<ColorSlot> colorSlots;
    //All created UI elements for slots.
    [SerializeField] List<RectTransform> slotList;
    //Positions of all UI Containers for slots.
    List<Vector2> slotPositions = new List<Vector2>();
    //Scale of all UI containers for slots
    List<Vector3> slotScales = new List<Vector3>();
    //Different sets of slots used for different amount of color slots the player has.

    # region Setup
    /// <summary>
    /// When enabling the Player In game UI, set up the script.
    /// </summary>
    private void OnEnable() {
        //Fetch the players current colors.
        colorInventory = colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();;
        colorSlots = colorInventory.colorSlots;

        //Attach local functions to UnityActions.
        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;

        //fetch each colorSlots position and scale and save it.
        foreach(RectTransform rect in slotList) {
            slotPositions.Add(((RectTransform)rect.parent).anchoredPosition);
            slotScales.Add(rect.parent.transform.localScale);
        }

        //Init every color
        for(int i = 0; i < slotList.Count; i++) {
            colorSlots[i].Init(slotList[i].GetComponent<Image>());
        }
    }

    //When turning off UI, detatch UnityActions from local functions. 
    private void OnDisable() {
        colorInventory.onColorUpdated -= ColorUpdate;
        colorInventory.onSlotChanged -= ActiveColorChanged;
    }
    #endregion
    #region SlotMovement
/// <summary>
/// Moves the different containers for slots around and change scale when cycling colors according to direction. 
/// Q=Clockwise=1, E=CounterClockwise=-1.
/// Also keeps the slotList[] sorted so that the active color is slotList[0].
/// </summary>
/// <param name="dir"></param> Which direction the slots are rotating in.
    private void RotateSlots(int dir) {
        for(int i = 0; i < slotList.Count; i++) {
            int x = (i + dir+slotList.Count)%(slotList.Count);
            RectTransform rect = (RectTransform)slotList[i].parent;
            rect.anchoredPosition = slotPositions[x]; 
            rect.transform.localScale = slotScales[x];
        }

        if(dir == -1) { 
            RectTransform temp = slotList[0];
            slotList.Remove(slotList[0]);
            slotList.Insert(slotList.Count, temp);
        } else if(dir == 1) {
            RectTransform temp = slotList[slotList.Count-1];
            slotList.Remove(slotList[slotList.Count-1]);
            slotList.Insert(0, temp);
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
       RotateSlots(dir*-1);
    }
    #endregion
}