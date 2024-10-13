using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Steamworks;

/// <summary>
/// Keeps track of the colors that the player has gathered. 
/// </summary>
public class ColorInventory : MonoBehaviour
{
    int startColorSlots; //The number of starting color slots that the player has
    [SerializeField] float colorBuff;

    /// <summary>
    /// The existing color slots that the player have
    /// </summary>
    [SerializeField] public List<ColorSlot> colorSlots;

    /// <summary>
    /// The index of the active color
    /// </summary>
    [SerializeField] public int activeSlot;

    [SerializeField] public ColorSpell defaultSpell;
    [SerializeField] InputActionReference changeRightActions;
    [SerializeField] InputActionReference pickUpAction;
    [SerializeField] public Material defaultColor;
    [SerializeField] InputActionReference removeColorAction;
    [SerializeField] int rainbowExtraDrain;
    public Dictionary<GameColor, float> colorBuffs = new Dictionary<GameColor, float>();
    SpellPickup pickUpSpell = null;

    public int blockDrainColor = 0;

    #region Actions for UI
    
    /// <summary>
    /// Called when the active color slot is changed
    /// </summary>
    public UnityAction<int> onSlotChanged;

    /// <summary>
    /// Called when the the color in the active color slot is updated
    /// </summary>
    public UnityAction onColorUpdated;

    /// <summary>
    /// Called when the number of color slots is changed
    /// </summary>
    public UnityAction onColorSlotsChanged;
    
    /// <summary>
    /// Called when the color spell of a color slot is changed. The int is the index of the chaged slot
    /// </summary>
    public UnityAction<int> onColorSpellChanged;

    /// <summary>
    /// Called whenever a color spell gets in range our out of range of being picked up.
    /// Sends a bool as parameter; if bool == true the spell got in range and if bool == false the spell left the range
    /// </summary>
    public UnityAction<bool> onSpellPickupInRange;
    
    private System.Action<InputAction.CallbackContext> divideColorHandler;

    #endregion

    #region Setup

    void Start()
    {
        startColorSlots = colorSlots.Count;
    }   

    UnityAction<int> slotChangedBrush; 

    void OnEnable()
    {
        slotChangedBrush = (int dir) => {updateBrushColor();}; 
        changeRightActions.action.performed += (dir) => {RotateActive((int)dir.ReadValue<float>()); };
        onColorUpdated += updateBrushColor;
        onSlotChanged += slotChangedBrush;
        pickUpAction.action.performed += PickUp;
        divideColorHandler = (InputAction.CallbackContext ctx) => DivideColor();
        removeColorAction.action.performed += divideColorHandler;
    }

    void OnDisable()
    {
        changeRightActions.action.performed -= (dir) => {RotateActive((int)dir.ReadValue<float>()); };
        onColorUpdated -= updateBrushColor;
        onSlotChanged -= slotChangedBrush;
        
        pickUpAction.action.performed -= PickUp;
        removeColorAction.action.performed -= divideColorHandler;
    }

    #endregion

    #region Active color slot

    /// <summary>
    /// Changes the active color slot by rotating in a direction
    /// </summary>
    /// <param name="dir"></param>
    public void RotateActive(int dir)
    {
        activeSlot = (colorSlots.Count+activeSlot+dir)%colorSlots.Count;
        onSlotChanged?.Invoke(dir);
    }

    /// <summary>
    /// Returns the color effect from the active color slot and decreases its charge with 1
    /// </summary>
    /// <returns></returns>
    public GameColor UseActiveColor()
    {
        if(ActiveSlot().charge > 0)
        {   
            GameColor ret = ActiveSlot().gameColor;

            if (Random.Range(0, 100) > blockDrainColor)
            {
                int charge = ActiveSlot().charge - 1;
                if (ActiveSlot().gameColor.name == "Rainbow")
                    charge -= rainbowExtraDrain;
                if(charge < 0) charge = 0;
                ActiveSlot().SetCharge(charge);

                onColorUpdated?.Invoke();
            }

            return ret;
            
        }
        return null;
    }

    /// <summary>
    /// Returns the active color slot
    /// </summary>
    /// <returns></returns>
    private ColorSlot ActiveSlot()
    {
        return colorSlots[activeSlot];
    }

    /// <summary>
    /// Returns the color effect from the active color slot
    /// </summary>
    public GameColor CheckActveColor()
    {
        if(ActiveSlot().charge > 0)
        {
            return ActiveSlot().gameColor;
        }
        return null;
    }

    /// <summary>
    /// Returns the colorspell for the active slot or the default spell if no such spell is attached
    /// </summary>
    /// <returns></returns>
    public ColorSpell GetActiveColorSpell()
    {
        if(ActiveSlot().colorSpell)
        {
            return ActiveSlot().colorSpell;
        }
        return defaultSpell;
    }

    #endregion

    #region Color buff

    /// <summary>
    /// Returns the color buff of the specified color. Returns 1 if no color buff exists.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public float GetColorBuff(GameColor color)
    {
        float buff = 0;
        foreach (ColorSlot slot in colorSlots)
        {
            if(slot.gameColor == color && slot.charge == slot.maxCapacity) 
            {
                buff += colorBuff;
            }
        }

        if(colorBuffs.ContainsKey(color))
            buff += colorBuffs[color];

        return buff;
    }

    /// <summary>
    /// Gets the color buff for the active slots color- Returns 1 if no color buff exists.
    /// </summary>
    /// <returns></returns>
    public float GetColorBuff()
    {
        return GetColorBuff(ActiveSlot().gameColor);
    }
    #endregion

    #region Divide color action
    
    private void DivideColor()
    {
        GameColor gameColor = ActiveSlot().gameColor;
        int amount = ActiveSlot().charge;
        if(gameColor == null || amount <= 0) return;

        ActiveSlot().RemoveColor();
        
        int rootAmount = amount/gameColor.rootColors.Length;
        int existingRootAmount = 0;
        foreach(GameColor rootColor in gameColor.rootColors)
        {
            for(int i = 1; i < colorSlots.Count; i++)
            {
                int check = (activeSlot+i)%colorSlots.Count;
                if(colorSlots[check].gameColor != null && colorSlots[check].charge > 0 && colorSlots[check].gameColor.ContainsRootColor(rootColor) && !colorSlots[check].IsFilledMax())
                {
                    existingRootAmount++;
                    break;
                }
            }
        }

        if(existingRootAmount <= 0)
        {
            onColorUpdated?.Invoke();
            return;
        }

        amount -= (rootAmount * (gameColor.rootColors.Length - existingRootAmount));

        HashSet<ColorSlot> fillableSlots = new HashSet<ColorSlot>();
        foreach(GameColor rootColor in gameColor.rootColors)
        {
            for (int i = 1; i < colorSlots.Count; i++)
            {
                int check = (activeSlot+i)%colorSlots.Count;
                if(colorSlots[check].gameColor == null || colorSlots[check].charge <= 0) continue;
                if(colorSlots[check].gameColor.ContainsRootColor(rootColor) && colorSlots[check].gameColor)
                {
                    if(!fillableSlots.Contains(colorSlots[check]) && !colorSlots[check].IsFilledMax()) fillableSlots.Add(colorSlots[check]);
                }
            }
        }

        amount = amount/fillableSlots.Count;
        if(amount < 1) amount = 1;
        foreach (ColorSlot slot in fillableSlots)
        {
            slot.AddCharge(amount);
        }

        onColorUpdated?.Invoke();
    }

    #endregion

    #region Add and remove colors

    public void AddColorOrbColor(GameColor gameColor, int amount)
    {
        int rootAmount = amount/gameColor.rootColors.Length;
        int existingRootAmount = 0;
        foreach(GameColor rootColor in gameColor.rootColors)
        {
            for(int i = 0; i < colorSlots.Count; i++)
            {
                int check = (activeSlot+i)%colorSlots.Count;
                if(colorSlots[check].gameColor != null && colorSlots[check].charge > 0 && colorSlots[check].gameColor.ContainsRootColor(rootColor) && !colorSlots[check].IsFilledMax())
                {
                    existingRootAmount++;
                    break;
                }
            }
        }

        if(existingRootAmount <= 0)
        {
            onColorUpdated?.Invoke();
            return;
        }

        amount -= (rootAmount * (gameColor.rootColors.Length - existingRootAmount));

        HashSet<ColorSlot> fillableSlots = new HashSet<ColorSlot>();
        foreach(GameColor rootColor in gameColor.rootColors)
        {
            for (int i = 0; i < colorSlots.Count; i++)
            {
                int check = (activeSlot+i)%colorSlots.Count;
                if(colorSlots[check].gameColor == null || colorSlots[check].charge <= 0) continue;
                if(colorSlots[check].gameColor.ContainsRootColor(rootColor) && colorSlots[check].gameColor)
                {
                    if(!fillableSlots.Contains(colorSlots[check]) && !colorSlots[check].IsFilledMax()) fillableSlots.Add(colorSlots[check]);
                }
            }
        }

        amount = amount/fillableSlots.Count;
        if(amount < 1) amount = 1;
        foreach (ColorSlot slot in fillableSlots)
        {
            slot.AddCharge(amount);
        }

        onColorUpdated?.Invoke();
    }

    /// <summary>
    /// Adds color to the active color slott. Mixes the colors if color already exist there
    /// </summary>
    /// <param name="color"></param>
    /// <param name="amount"></param>
    public void AddColor(GameColor color, int amount)
    {
        if(color == null) return;
        ColorSlot fillSlot = null;
        
        fillSlot = ActiveSlot();

        /*
        if(ActiveSlot().IsEmpty() || ActiveSlot().gameColor == color)
            fillSlot = ActiveSlot();
        else if(!ActiveSlot().IsEmpty())
        {
            bool hasRoots = true;
            foreach(GameColor rootColor in color.rootColors)
            {
                if(!ActiveSlot().gameColor.ContainsRootColor(rootColor))
                {
                    hasRoots = false;
                    break;
                }
            }

            if(hasRoots) fillSlot = ActiveSlot();
        }

        if(fillSlot == null || fillSlot.IsFilledMax())
        {
            for (int i = 1; i < colorSlots.Count; i++)
            {
                int pick = (activeSlot+i)%colorSlots.Count;
                if(colorSlots[pick].gameColor == color && colorSlots[pick].charge > 0)
                {
                    if(fillSlot == null || fillSlot.IsFilledMax())
                    {
                        fillSlot = colorSlots[pick];
                    }
                }
            }

            if(fillSlot == null) fillSlot = ActiveSlot();
        } */

        // Slot to Fill is now chosen

        GameColor setColor;
        //if(ActiveSlot().gameColor?.name == "Rainbow" && ActiveSlot().charge > 0) return;
        
        if(fillSlot.charge > 0)
            setColor = fillSlot.gameColor.MixColor(color);
        else
            setColor = color;
        int setAmount = fillSlot.charge + amount;
        setAmount = (setAmount > fillSlot.maxCapacity)?  fillSlot.maxCapacity : setAmount;

        fillSlot.SetCharge(setAmount);
        fillSlot.SetGameColor(setColor);

        onColorUpdated?.Invoke();
    }

    /// <summary>
    /// Remove all color from all color slots
    /// </summary>
    public void RemoveAllColors()
    {
        foreach (ColorSlot item in colorSlots)
        {
            item.RemoveColor();
        }

        onColorUpdated?.Invoke();
    }

    /// <summary>
    /// Removes the color from the active color slot
    /// </summary>
    public void RemoveActiveColor()
    {
        ActiveSlot().RemoveColor();
        onColorUpdated?.Invoke();
    }

    private void updateBrushColor()
    {
        // brush.
        GetComponent<SpriteRenderer>().material = ActiveSlot().charge > 0 ? ActiveSlot().gameColor.colorMat : defaultColor;
    }

    #endregion

    #region Change color spells
    public void PickUp(InputAction.CallbackContext ctx)
    {
        if(pickUpSpell == null) return;
        if(pickUpSpell.GetNeedsPayement())
        {
            if(GetComponent<ItemInventory>().PayCost(pickUpSpell.GetSpell().spellCost))
            {
                pickUpSpell.PickedUp();
            }
        } else
        {
            pickUpSpell.PickedUp();
        }
    }

    /// <summary>
    /// Enables a spell to be picked up
    /// </summary>
    /// <param name="spell"></param>
    public void EnablePickUp(SpellPickup spell)
    {
        if(pickUpSpell != null)
        {
            pickUpSpell.OnTriggerExit2D(GetComponent<Collider2D>());
        }

        pickUpSpell = spell;
        onSpellPickupInRange?.Invoke(true);
    }

    /// <summary>
    /// Disables a spell from being picked up
    /// </summary>
    /// <param name="spell"></param>
    public void DisablePickUp (SpellPickup spell) 
    {
        pickUpSpell = null;  
        onSpellPickupInRange?.Invoke(false);
    }

    /// <summary>
    /// Chagnes the color spell for the active slot
    /// </summary>
    /// <param name="newSpell"></param>
    public void ChangeActiveSlotColorSpell(ColorSpell newSpell)
    {
        ActiveSlot().colorSpell = newSpell;
        onColorSpellChanged?.Invoke(activeSlot);
    }

    /// <summary>
    /// Returns the color spell of the specifed slot. Returns the default spell if no spell is specified for that slot
    /// </summary>
    /// <param name="index"> The index of the color slot</param>
    /// <returns></returns>
    public ColorSpell GetColorSpell(int index)
    {
        if(colorSlots[index].colorSpell == null) 
            return defaultSpell;
        return colorSlots[index].colorSpell;
    }

    /// <summary>
    /// Changes the color spell of the specified slot
    /// </summary>
    /// <param name="index"> The index of the slot that the color slot should change on</param>
    /// <param name="newSpell"></param>
    public void ChangeColorSpell(int index, ColorSpell newSpell)
    {
        if(index < 0 || index > colorSlots.Count) 
        {
            Debug.LogWarning("Changed spell of color slot that didnt exist. Color slots available = " + colorSlots.Count + " index = " + index);
            return;
        }
        colorSlots[index].colorSpell = newSpell;
        onColorSpellChanged?.Invoke(index);
    }

    /// <summary>
    /// Resets all color spells to their default state
    /// </summary>
    public void ResetAllColorSpells()
    {
        for (int i = 0; i < colorSlots.Count; i++)
        {
            ChangeColorSpell(i, null);
        }
    }

    #endregion

    #region Add and remove color slots

    /// <summary>
    /// Adds a color slot. Can't add more color slots than the UI has specified
    /// </summary>
    public void AddColorSlot()
    {
        if(colorSlots.Count >= 6) return;
        while(activeSlot != 0) {
            RotateActive(1);
        }
        colorSlots.Add(new ColorSlot());
        colorSlots[colorSlots.Count-1].maxCapacity = colorSlots[0].maxCapacity;
        onColorSlotsChanged?.Invoke();
    }

    /// <summary>
    /// Removes the last color slot
    /// </summary>
    public void RemoveColorSlot()
    {
        if(colorSlots.Count <= 3) return;
        colorSlots.RemoveAt(colorSlots.Count-1);
        if(activeSlot >= colorSlots.Count) 
            activeSlot = colorSlots.Count-1;
        onColorSlotsChanged?.Invoke();
    }

    /// <summary>
    /// Removes all color, resets all color spells and resets the ammount of slots to the initial state.
    /// </summary>
    public void ResetColorSlots()
    {
        RemoveAllColors();
        ResetAllColorSpells();
        while(colorSlots.Count > startColorSlots)
            RemoveColorSlot();
        onColorSlotsChanged?.Invoke();
        onColorUpdated?.Invoke();
    }

    #endregion

}

#region Color slot

/// <summary>
/// Defines the traits of a color slot
/// </summary>
[System.Serializable]
public class ColorSlot
{
    [SerializeField] float imageScale;
    [SerializeField] public int maxCapacity = 6;
    [SerializeField] public int charge;
    [SerializeField] public GameColor gameColor;
    [SerializeField] public ColorSpell colorSpell;
    public void Init(Image setImage)
    {
        SetGameColor(gameColor);
        SetCharge(charge);
    }

    public void SetCharge(int set)
    {
        charge = set;
        if(charge > maxCapacity)
        charge = maxCapacity;
    }

    public void AddCharge(int addCharge)
    {
        SetCharge(charge + addCharge);
    }
    public void SetGameColor(GameColor set) 
    {
        gameColor = set;
    }

    public void RemoveColor()
    {
        SetCharge(0);
    }

    public bool IsFilledMax()
    {
        return charge >= maxCapacity;
    }

    public bool IsEmpty()
    {
        return (charge <= 0 || gameColor == null);
    }
}

# endregion