using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// Script for handling all the UI features of the ColorSlotUI.
/// </summary>
public class ColorSlotController : MonoBehaviour
{
    ColorInventory colorInventory;
    [SerializeField] UIController uiController;

    [SerializeField] Color backBottleTint;

    //Color slots the player currently has.
    List<ColorSlot> colorSlots;

    //All created UI elements for slots.
    [SerializeField] List<RectTransform> slotList;
    

    //Positions of all UI Containers for slots.
    List<Vector2> slotPositions = new List<Vector2>();

    //Scale of all UI containers for slots
    List<Vector3> slotScales = new List<Vector3>();

    //List of booleans to signal if each bottle is full
    List<bool> bottleFull = new List<bool>();
    

    // Spline to animate the filling effect of the color slots
    public AnimationCurve fillCurve;

    [Space(10)]
    // Spline to determine how "square" the movement of the bottles is.
    // A straight line will make the bottles go directly to their new position
    // A very accentuated curve will make the bottles go in a square pattern
    // Very mild curve is recommnded to make them go in a circular pattern
    [SerializeField] AnimationCurve movementCurve;
    // The rate at which the bottles will move to their new position. Inverse cubic spline will make it look snappy
    [SerializeField] AnimationCurve movementRateCurve;
    [SerializeField] float rotationTime;

    private Coroutine movementCoroutine;

    [ItemCanBeNull] private List<Coroutine> activeCoroutines = new List<Coroutine>();

    [SerializeField] public Sprite[] FullBottleEffectSprites;

    private List<Slider> spellsOnCoolDown = new List<Slider>();

    # region Setup
    /// <summary>
    /// When enabling the Player In game UI, set up the script.
    /// </summary>
    private void OnEnable()
    {
        //Fetch the players current colors.
        colorInventory = colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        colorSlots = colorInventory.colorSlots;

        //Attach local functions to UnityActions.
        colorInventory.onColorUpdated += ColorUpdate;
        colorInventory.onSlotChanged += ActiveColorChanged;
        colorInventory.onColorSpellChanged += BottleChanged;
        colorInventory.onCoolDownSet += StartCoolDownSlider;
        uiController.UILoaded += UpdateAllSprites;
        uiController.ColorSlotAmountChanged += UpdateAllSprites;

        //fetch each colorSlots position and scale and save it.
        foreach(RectTransform rect in slotList) {
            slotPositions.Add(rect.anchoredPosition);
            slotScales.Add(rect.transform.localScale);
            bottleFull.Add(false);
        }
        
        //Init every color
        var materials = Resources.LoadAll<Material>("Bottles/Materials/Body");
        var capMaterials = Resources.LoadAll<Material>("Bottles/Materials/Cap");
        for(int i = 0; i < slotList.Count; i++) {
            Image frameImage = slotList[i].GetChild(0).GetChild(0).GetComponent<Image>();
            frameImage.material = materials[i];
            ColorSlot slot = colorSlots[i];
            frameImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color"));
            frameImage.material.SetFloat("_fill", slot.charge / (float) slot.maxCapacity);
            
            Image capImage = slotList[i].GetChild(0).GetChild(1).GetComponent<Image>();
            capImage.material = capMaterials[i];
            capImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.colorMat.color : colorInventory.defaultColor.GetColor("_Color"));
            capImage.material.SetColor("_PlainColor", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color"));
            capImage.material.SetFloat("_Alpha", 0);
            capImage.material.SetFloat("_BloomPower", 0);

            Image capEffect = slotList[i].GetChild(0).GetChild(2).GetComponent<Image>();
            capEffect.sprite = FullBottleEffectSprites[0];
            capEffect.gameObject.SetActive(false);
            
            activeCoroutines.Add(null);
            BottleChanged(i);
        }
    }

    //When turning off UI, detatch UnityActions from local functions. 
    private void OnDisable() {
        colorInventory.onColorUpdated -= ColorUpdate;
        colorInventory.onSlotChanged -= ActiveColorChanged;
        colorInventory.onColorSpellChanged -= BottleChanged;
        uiController.UILoaded -= UpdateAllSprites;
        uiController.ColorSlotAmountChanged -= UpdateAllSprites;
        colorInventory.onCoolDownSet -= StartCoolDownSlider;
    }
    #endregion
    #region SlotMovement
/// <summary>
/// Moves the different containers for slots around and change scale when cycling colors according to direction. 
/// Q=CounterClockwise=1, E=Clockwise=-1.
/// Also keeps the slotList[] sorted so that the active color is slotList[0].
/// </summary>
/// <param name="dir"></param> Which direction the slots are rotating in.
    private IEnumerator RotateSlots(int dir) {
        bool middleOfAnim = false;
        bool bottleChangedDone = false;
        float halfTime = rotationTime / 2;
        // AnimationCurve
        for (float time = 0; time < rotationTime; time += Time.deltaTime)
        {
            float splineValue = time / rotationTime;
            if (time > halfTime)
                middleOfAnim = true;
            for (int i = 0; i < slotList.Count; i++)
            {
                int trueIndex = (i + slotList.Count - colorInventory.activeSlot) % slotList.Count;
                int currIndex = (trueIndex + slotList.Count + dir) % slotList.Count;
                RectTransform rect = slotList[i];
                // We get two values of the curve, the normal and the inverse and reversed
                float value1 = movementCurve.Evaluate(movementRateCurve.Evaluate(splineValue));
                float value2 = 1 - movementCurve.Evaluate(1 - movementRateCurve.Evaluate(splineValue));
                // Depending on the position, the bottle will accelerate at a certain rate for each bottle
                // This creates the notion of circular movement
                rect.anchoredPosition = new Vector2(
                    Mathf.Lerp(slotPositions[currIndex].x, slotPositions[trueIndex].x, currIndex % 2 == 1 ? value1 : value2),
                    Mathf.Lerp(slotPositions[currIndex].y, slotPositions[trueIndex].y, currIndex % 2 == 0 ? value1 : value2)
                );

                rect.transform.localScale = Vector2.Lerp(
                    slotScales[currIndex], 
                    slotScales[trueIndex], 
                    currIndex % 2 == 0 ? value1 : value2);

                // We want the change of resolution to happen in the middle of the movement, so it's not noticeable
                // But it must only happen once
                if (middleOfAnim && !bottleChangedDone)
                {
                    BottleChanged(i, trueIndex);
                }
            }
            bottleChangedDone = middleOfAnim;
            yield return new WaitForEndOfFrame();
        }

        // Bottles don't always move all the way, so we do one last update to set them in their exact place
        for(int i = 0; i < slotList.Count; i++) {
            int trueIndex = (i + slotList.Count - colorInventory.activeSlot) % slotList.Count;
            RectTransform rect = slotList[i];
            rect.anchoredPosition = slotPositions[trueIndex];
            rect.transform.localScale = slotScales[trueIndex];

            if (!bottleChangedDone)
                BottleChanged(i, trueIndex);
        }
    }

    #endregion

    #region UnityActions
    //When a color is updated, call this.
    private void ColorUpdate(int index) {
        Image frameImage = slotList[index].GetChild(0).GetChild(0).GetComponent<Image>();
        Image capImage = slotList[index].GetChild(0).GetChild(1).GetComponent<Image>();
        ColorSlot slot = colorInventory.colorSlots[index];
        
        frameImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color")); 
        capImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.colorMat.GetColor("_Color") : colorInventory.defaultColor.GetColor("_Color"));
        capImage.material.SetColor("_PlainColor", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color"));

        if (capImage.material.GetFloat("_Alpha") == 0f && slot.charge > 0)
            StartCoroutine(setActivateCap(capImage, slot, true));
        else if (capImage.material.GetFloat("_Alpha") == 1f && slot.charge == 0)
            StartCoroutine(setActivateCap(capImage, slot, false));

        if (activeCoroutines[index] != null)
            StopCoroutine(activeCoroutines[index]);
        activeCoroutines[index] = StartCoroutine(fillSlotGradually(frameImage, slot));
    }

    private void ColorUpdate() {

        for (int i = 0; i < colorInventory.colorSlots.Count; i++)
        {
            ColorUpdate(i);
        }
    }

    private IEnumerator  fillSlotGradually(Graphic frame, ColorSlot color)
    {
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

    private IEnumerator setActivateCap(Graphic cap, ColorSlot color, bool activate)
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            cap.material.SetFloat("_Alpha", activate ? i : 1 - i);
            yield return new WaitForSeconds(0.001f);
        }
        cap.material.SetFloat("_Alpha", activate ? 1 : 0);
    }

    private IEnumerator initCapFull(Image overflow, ColorSlot color)
    {
        // Get fullBottle spritesheet from Resources/Bottles
        Sprite fullBottle = Resources.Load<Sprite>("Bottles/fullBottle");

        overflow.color = color.gameColor.plainColor;
        overflow.gameObject.SetActive(true);
        foreach (var sprite in FullBottleEffectSprites)
        {
            overflow.sprite = sprite;
            yield return new WaitForSeconds(0.05f);
        }
        overflow.gameObject.SetActive(false);
        overflow.sprite = FullBottleEffectSprites[0];
    }

    /// <summary>
    /// When active color has changed, rotate UI according to direction.
    /// </summary>
    /// <param name="dir"></param> Direction to rotate in.
    private void ActiveColorChanged(int dir)
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(RotateSlots(dir));
    }

    /// <summary>
    /// Updates bottle sprite at specified index by fetching sprites from spell. 
    /// </summary>
    /// <param name="index"></param> which slot changed.
    private void BottleChanged(int index, int pos) {
        Image bottle = slotList[index].GetChild(0).GetComponent<Image>();
        Image bottleMask = slotList[index].GetChild(0).GetChild(0).GetComponent<Image>();
        Image capMask = slotList[index].GetChild(0).GetChild(1).GetComponent<Image>();
        BottleSprite bottleSprite = colorInventory.GetColorSpell(index).GetBottleSprite();
        if(pos == 0) {
            bottle.sprite = bottleSprite.bigSprite;
            bottleMask.sprite = bottleSprite.bigSpriteMask;
            capMask.sprite = bottleSprite.bigSpriteCapMask;
        } else if(pos == 1 || pos == slotList.Count-1) {
            bottle.sprite = bottleSprite.mediumSprite;
            bottleMask.sprite = bottleSprite.mediumSpriteMask;
            capMask.sprite = bottleSprite.mediumSpriteCapMask;
        } else {
            bottle.sprite = bottleSprite.smallSprite;
            bottleMask.sprite = bottleSprite.smallSpriteMask;
            capMask.sprite = bottleSprite.smallSpriteCapMask;
        }

        slotList[index].GetComponentInChildren<Slider>().GetComponentInChildren<Image>().sprite = bottle.sprite;

        foreach (Image image in slotList[index].GetComponentsInChildren<Image>())
        {
            if(pos == 0)
            {
                image.color = Color.white;
            } else
            {
                image.color = backBottleTint;
            }
        }
    }

    private void BottleChanged(int index) {
        BottleChanged(index, (index + slotList.Count - colorInventory.activeSlot) % slotList.Count);
    }

    /// <summary>
    /// Updates all bottles, their caps and colors.
    /// </summary>
    private void UpdateAllSprites() {
        for (int i = 0; i < slotList.Count; i++){
            ColorUpdate(i);
            BottleChanged(i);
        }
    }

    /// <summary>
    /// Add the CoolDown indicator to a list when a spell is used. 
    /// </summary>
    /// <param name="time"></param>
    private void StartCoolDownSlider(float time)
    {
        if (colorInventory.activeSlot >= slotList.Count) return;
        if (slotList[colorInventory.activeSlot] == null) return; 
        Slider slide = slotList[colorInventory.activeSlot].GetComponentInChildren<Slider>();
        if (slide == null) return;    
        if (spellsOnCoolDown.Contains(slide)) return;
        slide.maxValue = time;
        slide.value = time;
        spellsOnCoolDown.Add(slide);
    }
    private void Update()
    {
        // Get sinewave value based on time
        float sinewave = Mathf.Sin(Time.time * 2f) * 0.3f + 0.7f;
        for (int i = 0; i < slotList.Count; i++)
        {
            var cap = slotList[i].GetChild(0).GetChild(1).GetComponent<Image>();
            if (colorSlots[i].charge != colorSlots[i].maxCapacity)
            {
                if (bottleFull[i])
                    bottleFull[i] = false;
                cap.material.SetFloat("_BloomPower", 0);
            }
            else
            {
                if (!bottleFull[i])
                {
                    ColorSlot slot = colorInventory.colorSlots[i];
                    Image capEffect = slotList[i].GetChild(0).GetChild(2).GetComponent<Image>();
                    StartCoroutine(initCapFull(capEffect, slot));
                    bottleFull[i] = true;
                }
                cap.material.SetFloat("_BloomPower", sinewave);
            }
        }

        //Updates cooldown display
        if (spellsOnCoolDown.Count > 0)
        {
            foreach (Slider slide in spellsOnCoolDown.ToList())
            {
                slide.value -= Time.deltaTime;
                if (slide.value <= 0)
                {
                    slide.value = 0;
                    spellsOnCoolDown.Remove(slide);
                }
            }
        }
        

    }

    #endregion
}