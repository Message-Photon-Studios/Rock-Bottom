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
    // Spline to animate the filling effect of the color slots
    public AnimationCurve fillCurve;


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
            slotPositions.Add(((RectTransform)rect).anchoredPosition);
            slotScales.Add(rect.transform.localScale);
        }
        
        //Init every color
        var materials = Resources.LoadAll<Material>("Bottles/Materials");
        for(int i = 0; i < slotList.Count; i++) {
            Image frameImage = slotList[i].GetChild(0).GetChild(0).GetComponent<Image>();
            frameImage.material = materials[i];
            ColorSlot slot = colorSlots[i];
            if (slot.gameColor != null)
                frameImage.material.SetColor("_Color", slot.gameColor.plainColor);
            else
                frameImage.material.SetColor("_Color", colorInventory.defaultColor.GetColor("_Color"));
            frameImage.material.SetFloat("_fill", slot.charge / slot.maxCapacity);
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
/// Q=CounterClockwise=1, E=Clockwise=-1.
/// Also keeps the slotList[] sorted so that the active color is slotList[0].
/// </summary>
/// <param name="dir"></param> Which direction the slots are rotating in.
    private void RotateSlots(int dir) {
        for(int i = 0; i < slotList.Count; i++) {
            int trueIndex = (i + slotList.Count - colorInventory.activeSlot) % slotList.Count;
            Debug.Log(trueIndex);
            RectTransform rect = slotList[i];
            rect.anchoredPosition = slotPositions[trueIndex];
            rect.transform.localScale = slotScales[trueIndex];
        }
    }
    #endregion

    #region UnityActions
    //When a color is updated, call this.
    private void ColorUpdate() {
        Image frameImage = slotList[colorInventory.activeSlot].GetChild(0).GetChild(0).GetComponent<Image>();
        ColorSlot slot = colorInventory.colorSlots[colorInventory.activeSlot];
        if (slot.gameColor != null)
                frameImage.material.SetColor("_Color", slot.gameColor.plainColor);
            else
                frameImage.material.SetColor("_Color", colorInventory.defaultColor.GetColor("_Color"));
        StartCoroutine(fillSlotGradually(frameImage));
    }

    private IEnumerator  fillSlotGradually(Image frame)
    {
        ColorSlot color = colorSlots[colorInventory.activeSlot];
        float prevValue = frame.material.GetFloat("_fill");
        float newValue = color.charge / (float) color.maxCapacity;

        for (float i = 0; i < 1; i += 0.01f)
        {
            float curvePoint = fillCurve.Evaluate(i);
            float value = prevValue + ((newValue - prevValue) * curvePoint);
            frame.material.SetFloat("_fill", value);
            yield return new WaitForSeconds(0.005f);
        }
        frame.material.SetFloat("_fill", color.charge / (float) color.maxCapacity);
    }

    /// <summary>
    /// When active color has changed, rotate UI according to direction.
    /// </summary>
    /// <param name="dir"></param> Direction to rotate in.
    private void ActiveColorChanged(int dir) {
       RotateSlots(dir);
    }
    #endregion
}