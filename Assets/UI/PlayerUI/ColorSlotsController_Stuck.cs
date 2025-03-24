using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ColorSlotsController_Stuck : MonoBehaviour
{
    ColorInventory colorInventory;
    [SerializeField] UIController uiController;

    //Color slots the player currently has.
    List<ColorSlot> colorSlots;

    //All created UI elements for slots.
    [SerializeField] List<RectTransform> slotList;
    

    //List of booleans to signal if each bottle is full
    List<bool> bottleFull = new List<bool>();
    

    // Spline to animate the filling effect of the color slots
    public AnimationCurve fillCurve;

    [ItemCanBeNull] private List<Coroutine> activeCoroutines = new List<Coroutine>();

    [SerializeField] public Sprite[] FullBottleEffectSprites;

    private List<Slider> spellsOnCoolDown = new List<Slider>();
    private List<(ColorSlot, Image)> spellsWithchargesOnCD = new List<(ColorSlot, Image)>();

    [SerializeField] public Sprite[] ChargesSprites;
    [SerializeField] public Sprite[] ChargesMaskSprites;

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
        colorInventory.onColorSpellChanged += BottleChanged;
        colorInventory.onCoolDownSet += StartCoolDownSlider;
        colorInventory.onSpellChargeChange += UpdateAllSprites;
        uiController.UILoaded += UpdateAllSprites;
        uiController.ColorSlotAmountChanged += UpdateAllSprites;

        //fetch each colorSlots position and scale and save it.
        foreach(RectTransform rect in slotList) {
            bottleFull.Add(false);
        }
        
        //Init every color
        var materials = Resources.LoadAll<Material>("Bottles/Materials/Body");
        var capMaterials = Resources.LoadAll<Material>("Bottles/Materials/Cap");
        var chargeMaterials = Resources.LoadAll<Material>("Bottles/Materials/Charge");
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

            Image chargeImage = slotList[i].GetChild(2).GetChild(0).GetComponent<Image>();
            chargeImage.material = chargeMaterials[i];
            chargeImage.material.SetColor("_Color", slot.gameColor != null && slot.charge > 0 ? slot.gameColor.colorMat.color : colorInventory.GetEmptyBottleColor().plainColor);
            chargeImage.material.SetColor("_PlainColor", slot.gameColor != null && slot.charge > 0 ? slot.gameColor.plainColor : colorInventory.GetEmptyBottleColor().plainColor);
            chargeImage.material.SetFloat("_BloomPower", 0);

            activeCoroutines.Add(null);
        }
        UpdateAllSprites();
    }

    //When turning off UI, detatch UnityActions from local functions. 
    private void OnDisable() {
        colorInventory.onColorUpdated -= ColorUpdate;
        colorInventory.onColorSpellChanged -= BottleChanged;
        colorInventory.onCoolDownSet -= StartCoolDownSlider;
        colorInventory.onSpellChargeChange -= UpdateAllSprites;
        uiController.UILoaded -= UpdateAllSprites;
        uiController.ColorSlotAmountChanged -= UpdateAllSprites;
        
    }
    #endregion

    #region UnityActions
    //When a color is updated, call this.
    private void ColorUpdate(int index) {
        Image frameImage = slotList[index].GetChild(0).GetChild(0).GetComponent<Image>();
        Image capImage = slotList[index].GetChild(0).GetChild(1).GetComponent<Image>();
        Image chargeImage = slotList[index].GetChild(2).GetChild(0).GetComponent<Image>();
        ColorSlot slot = colorInventory.colorSlots[index];
        
        frameImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color")); 
        capImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.colorMat.GetColor("_Color") : colorInventory.defaultColor.GetColor("_Color"));
        capImage.material.SetColor("_PlainColor", slot.gameColor != null ? slot.gameColor.plainColor : colorInventory.defaultColor.GetColor("_Color"));
        
        chargeImage.material.SetColor("_Color", slot.gameColor != null && slot.charge > 0 ? slot.gameColor.colorMat.GetColor("_Color") : colorInventory.GetEmptyBottleColor().plainColor);
        chargeImage.material.SetColor("_PlainColor", slot.gameColor != null && slot.charge > 0 ? slot.gameColor.plainColor : colorInventory.GetEmptyBottleColor().plainColor);

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
    /// Updates bottle sprite at specified index by fetching sprites from spell. 
    /// </summary>
    /// <param name="index"></param> which slot changed.
    private void BottleChanged(int index) {
        Image bottle = slotList[index].GetChild(0).GetComponent<Image>();
        Image bottleMask = slotList[index].GetChild(0).GetChild(0).GetComponent<Image>();
        Image capMask = slotList[index].GetChild(0).GetChild(1).GetComponent<Image>();
        BottleSprite bottleSprite = colorInventory.GetColorSpell(index).GetBottleSprite();

        if(index <= 1) {
            bottle.sprite = bottleSprite.bigSprite;
            bottleMask.sprite = bottleSprite.bigSpriteMask;
            capMask.sprite = bottleSprite.bigSpriteCapMask;
        } else {
            bottle.sprite = bottleSprite.mediumSprite;
            bottleMask.sprite = bottleSprite.mediumSpriteMask;
            capMask.sprite = bottleSprite.mediumSpriteCapMask;
        }
        
        slotList[index].GetComponentInChildren<Slider>().GetComponentInChildren<Image>().sprite = bottle.sprite;

        int chargesAmount = colorInventory.colorSlots[index].storedSpellCDs.Count;
        Image charges = slotList[index].GetChild(2).GetComponent<Image>();
        Image chargesMask = slotList[index].GetChild(2).GetChild(0).GetComponent<Image>();
        if(chargesAmount > 1) {
            charges.sprite = ChargesSprites[chargesAmount-1];
            charges.gameObject.SetActive(true);
            chargesMask.sprite = ChargesMaskSprites[chargesAmount-1];
            chargesMask.gameObject.SetActive(true);
            spellsWithchargesOnCD.Add((colorSlots[index], slotList[index].GetChild(2).GetChild(0).GetComponent<Image>()));

        } else {
            charges.gameObject.SetActive(false);
            chargesMask.gameObject.SetActive(false);
        }
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
    private void StartCoolDownSlider(List<float> spellCDList, float spellMaxCD, int index)
    {
        if (index >= slotList.Count) return;
        if (slotList[index] == null) return; 
        Slider slide = slotList[index].GetComponentInChildren<Slider>();
        if (slide == null) return;    
        if (spellsOnCoolDown.Contains(slide)) return;
        slide.maxValue = spellMaxCD;
        float min = spellMaxCD + Time.fixedTime;
        foreach(float cd in spellCDList) {
            if (cd < min) {
                min = cd;
            }
        } 
        spellsWithchargesOnCD.Add((colorSlots[index], slotList[index].GetChild(2).GetChild(0).GetComponent<Image>()));
        if(min < Time.fixedTime) {
            return;
        } else {
            slide.value = min-Time.fixedTime;
            spellsOnCoolDown.Add(slide);
        }   
    }

    /// <summary>
    /// Counts how many charges are on cooldown.
    /// </summary>
    /// <param name="spellCDList">spell cooldown list to count trough.</param>
    /// <returns> Amount of charges on cooldown </returns>
    private int ChargesOnCD(List<float> spellCDList) {
        int count = 0;
        foreach(float cd in spellCDList) {
            if(cd > Time.fixedTime) {
                count ++;
            }
        }
        return count;
    }
    
    private void Update()
    {
        // Get sinewave value based on time
        float sinewave = Mathf.Sin(Time.time * 2f) * 0.3f + 0.7f;
        for (int i = 0; i < slotList.Count; i++)
        {
            var cap = slotList[i].GetChild(0).GetChild(1).GetComponent<Image>();
            var charge = slotList[i].GetChild(2).GetChild(0).GetComponent<Image>();
            if (colorSlots[i].charge != colorSlots[i].maxCapacity)
            {
                if (bottleFull[i])
                    bottleFull[i] = false;
                cap.material.SetFloat("_BloomPower", 0);
                charge.material.SetFloat("_BloomPower", 0);
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
                charge.material.SetFloat("_BloomPower", sinewave);
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

        //Updates charges display on each spell currently active
        if(spellsWithchargesOnCD.Count > 0){
            foreach((ColorSlot slot, Image mask) in spellsWithchargesOnCD.ToList()) {
                int count = ChargesOnCD(slot.storedSpellCDs);
                if(count == slot.storedSpellCDs.Count) {
                    mask.gameObject.SetActive(false);
                } else {
                    mask.sprite = ChargesMaskSprites[slot.storedSpellCDs.Count -1 - count];
                    mask.gameObject.SetActive(true);
                    if(count == 0) {
                        spellsWithchargesOnCD.Remove((slot, mask));
                    }
                }
            }
        }
    }

    #endregion
}
