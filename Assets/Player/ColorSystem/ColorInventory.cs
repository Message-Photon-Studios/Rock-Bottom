using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Steamworks;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Keeps track of the colors that the player has gathered. 
/// </summary>
public class ColorInventory : MonoBehaviour
{
    int startColorSlots; //The number of starting color slots that the player has
    [SerializeField] float colorMaxBuff;

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
    [SerializeField] float minCD = 0.3f;
    [SerializeField] int maxStoredSpells = 5;
    [SerializeField] float routedSheildCost = 0.5f;
    [SerializeField] public bool lockSwapping = false;

    [SerializeField] GameColor emptyBottleColor;
    private Dictionary<GameColor, float> colorBuffs = new Dictionary<GameColor, float>();
    public Dictionary<string, int> spellsSpawned = new Dictionary<string, int>();
    Dictionary<string, int> spellTracker = new Dictionary<string, int>();
    SpellPickup pickUpSpell = null;
    ColorLibrary colorLib;
    Light2D playerLight;

    public int blockDrainColor = 0;
    private bool CanSwap = true;
    private float addetiveCDModifier = 0;
    private float multetiveCDModifier = 1;
    private float defaultBuff = 0;
    private int bonusSpells = 0;
    public bool balanceColors = false;
    public bool dontMixColor = false;
    public bool autoRotate = false;
    public bool chaosEnabled = false;
    public bool routedSheild = false;
    private float rngMax = 0;
    private float rngMin = 0;
    private float rngBuff = 0;
    private float colorMaxBonus = 0;
    private int colorMaxDamageBonus = 0;
    


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

    public UnityAction<List<float>, float, int> onCoolDownSet;

    /// <summary>
    /// Called whenever amount of charges on a spell is changed (usually trough an item).
    /// </summary>
    public UnityAction onSpellChargeChange;
    
    private System.Action<InputAction.CallbackContext> divideColorHandler;

    #endregion

    #region Setup

    UnityAction<int> slotChangedBrush; 
    void Start()
    {
        playerLight = GetComponentInChildren<Light2D>();
        startColorSlots = colorSlots.Count;
        slotChangedBrush = (int dir) => {updateBrushColor();}; 
        changeRightActions.action.performed += (dir) => {RotateActive((int)dir.ReadValue<float>()); };
        onColorUpdated += updateBrushColor;
        onSlotChanged += slotChangedBrush;
        ColorSpellImpact.onSpellImpact += SpellImactTrigger;
        SpellImactOnVelocity.onSpellImpact += SpellImactTrigger;
        pickUpAction.action.performed += PickUp;
        divideColorHandler = (InputAction.CallbackContext ctx) => DivideColor();
        removeColorAction.action.performed += divideColorHandler;
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerStats>().onPlayerDamaged += WhenDamaged;
        player.GetComponent<PlayerMovement>().onPlayerDash += DashSpells;
        player.GetComponent<PlayerMovement>().onPlayerDoubleJump += DoubleJumpSpells;
        colorLib = GameManager.instance.GetComponent<ColorLibrary>();

        foreach (ColorSlot colorSlot in colorSlots)
        {
            if(colorSlot.gameColor == null)
                colorSlot.SetGameColor(colorLib.GetRandomColor());
        }
        onColorUpdated?.Invoke();

        if(playerLight) playerLight.color = colorSlots[activeSlot].gameColor.lightTintColor;
    }

    void OnDisable()
    {
        changeRightActions.action.performed -= (dir) => {RotateActive((int)dir.ReadValue<float>()); };
        onColorUpdated -= updateBrushColor;
        onSlotChanged -= slotChangedBrush;
        
        pickUpAction.action.performed -= PickUp;
        removeColorAction.action.performed -= divideColorHandler;
        ColorSpellImpact.onSpellImpact -= SpellImactTrigger;
        SpellImactOnVelocity.onSpellImpact -= SpellImactTrigger;
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerStats>().onPlayerDamaged -= WhenDamaged;
        player.GetComponent<PlayerMovement>().onPlayerDash -= DashSpells;
        player.GetComponent<PlayerMovement>().onPlayerDoubleJump -= DoubleJumpSpells;
    }

    #endregion

    #region Active color slot

    /// <summary>
    /// Changes the active color slot by rotating in a direction
    /// </summary>
    /// <param name="dir"></param>
    public void RotateActive(int dir)
    {
        if (!CanSwap) return;
        if(lockSwapping) return;
        activeSlot = (colorSlots.Count+activeSlot+dir)%colorSlots.Count;
        onSlotChanged?.Invoke(dir);
        if (autoRotate) GetComponent<PlayerCombatSystem>().SpecialAttackAnimation();
    }

    public void DisableRotation()
    {
        CanSwap = false;
    }

    public void EnableRotation()
    {
        CanSwap = true;
    }

    /// <summary>
    /// Returns the color effect from the active color slot and decreases its charge with 1
    /// </summary>
    /// <returns></returns>
    public GameColor UseActiveColor()
    {
        return UseActiveColor(ActiveSlot());
    }

    public GameColor UseActiveColor(ColorSlot slot)
    {
        if(slot.charge > 0)
        {   
            GameColor ret = slot.gameColor;

            if (Random.Range(0, 100) > blockDrainColor)
            {
                int charge = slot.charge - 1;
                if (slot.gameColor.name == "Rainbow")
                    charge -= rainbowExtraDrain;
                if(charge < 0) charge = 0;
                slot.SetCharge(charge);

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
        return CheckActveColor(ActiveSlot());
    }

    public GameColor CheckActveColor(ColorSlot slot)
    {
        if (slot.charge > 0 && slot.gameColor != null)
        {
            return slot.gameColor;
        }
        return emptyBottleColor;
    }

    public void AddSpellSpawned(string spell, int i)
    {
        SetSpawnedSpellCount(spell, GetSpawnedSpellCount(spell) + i);
    }

    public void RemoveSpellSpawned(string spell, int i)
    {
        int count = GetSpawnedSpellCount(spell);
        count -= i;
        if (count < 0) count = 0;
        SetSpawnedSpellCount(spell, count);
    }

    public int GetSpawnedSpellCount(string spell)
    {
        if (spellsSpawned.ContainsKey(spell))
        {
            return spellsSpawned[spell];
        } else
        {
            spellsSpawned.Add(spell, 0);
            return 0;
        }
    }

    public void SetSpawnedSpellCount(string spell, int i)
    {
        if (spellsSpawned.ContainsKey(spell))
        {
            spellsSpawned[spell] = i;
        }
        else
        {
            spellsSpawned.Add(spell, i);
        }
    }

    public void AddBonusSpell(int add)
    {
        bonusSpells += add;
        foreach (ColorSlot slot in colorSlots)
        {
            ValidateCDlist(slot);
        }
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

    #region Cool Down

    /// <summary>
    /// Checks if the cooldown for a spell is done
    /// </summary>
    /// <returns></returns>
    public bool IsSpellReady()
    {
        return IsSpellReady(ActiveSlot());
    }

    public bool IsSpellReady(ColorSlot slot)
    {
        ValidateCDlist(slot);
        foreach (float coolDown in slot.storedSpellCDs)
        {
            if (coolDown <= Time.fixedTime && CheckCanSpawn(slot)) return true;
        }
        return false;
    }

    public bool CheckCanSpawn(ColorSlot slot)
    {
        ColorSpell spell = slot.colorSpell;
        if (spell == null) spell = defaultSpell;
        if (spell.maxSpawn <= 0) return true;
        if (spell.spawnKey == null || spell.spawnKey == "")
        {
            Debug.Log("Error: Spell is missing Spawn Key but is expected to be limited. Set a spawn key");
            return true;
        }
        int count = GetSpawnedSpellCount(spell.spawnKey);
        if (count < spell.maxSpawn) return true;
        return false;
    }

    public void ValidateCDlist(ColorSlot slot)
    {
        ColorSpell spell = slot.colorSpell;
        if (spell == null) spell = defaultSpell;
        int capacaty = spell.storedSpells + bonusSpells;
        if (capacaty > maxStoredSpells) capacaty = maxStoredSpells;
        if (capacaty <= 0) capacaty = 1;
        if (slot.storedSpellCDs.Count == 0)
        {
            slot.storedSpellCDs = CreateCDList(spell, 0);
            onSpellChargeChange?.Invoke();
        }
        if (slot.storedSpellCDs.Count != capacaty)
        {
            slot.storedSpellCDs = UpdateCDList(spell, slot.storedSpellCDs);
            onSpellChargeChange?.Invoke();
        }
    }

    /// <summary>
    /// Starts the cooldown
    /// </summary>
    /// <param name="time"></param>
    public void SetCoolDown(float time)
    {
        SetCoolDown(time, ActiveSlot());
    }

    public void SetCoolDown(float time, ColorSlot slot)
    {
        for (int i = 0; i < slot.storedSpellCDs.Count; i++)
        {
            if (slot.storedSpellCDs[i] <= Time.fixedTime)
            {
                slot.storedSpellCDs = SetCoolDownForIndex(slot.storedSpellCDs, i, time);
                break;
            }
        }
        onCoolDownSet?.Invoke(slot.storedSpellCDs, CalculateCD(time), colorSlots.IndexOf(slot));
    }

    public float CalculateCD(float time)
    {
        return (time - time * addetiveCDModifier) * multetiveCDModifier;
    }

    public List<float> SetCoolDownForIndex(List<float> list, int index, float time)
    {
        time = CalculateCD(time);
        if (time <= minCD) time = minCD;
        list[index] = 0;
        float max = Time.fixedTime;
        foreach (float cd in list)
        {
            if (max < cd) max = cd;
        }
        list[index] = time + max;
        return list;
    }

    public void MultiplyCDMultiplier(float multiply)
    {
        multetiveCDModifier *= multiply;
    }

    public void AddCDAddetive(float add)
    {
        addetiveCDModifier += add;
    }

    #endregion

    #region Color buff

    public void AddColorBuff(GameColor color, float addPower)
    {
        if(colorBuffs.ContainsKey(color)) 
        {
            colorBuffs[color] += addPower;
        } else
        {
            colorBuffs.Add(color, addPower);
        }

        GameManager.instance.tipsManager.DisplayTips("colorPower");
    }

    /// <summary>
    /// Returns the color buff of the specified color. Returns 1 if no color buff exists.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public float GetColorBuff(GameColor color)
    {
        if (color == null) return 0;
        float buff = 0;
        foreach (ColorSlot slot in colorSlots)
        {
            if((slot.gameColor == color || balanceColors) && slot.charge == slot.maxCapacity) 
            {
                buff += colorMaxBuff + colorMaxBonus;
            }
        }

        

        if (balanceColors)
        {
            foreach (KeyValuePair<GameColor, float> entry in colorBuffs)
            {
                buff += entry.Value;
            }
            buff = buff / 7;
        } else if (colorBuffs.ContainsKey(color))
            buff += colorBuffs[color];
        buff += defaultBuff + rngBuff;

        return buff;
    }

    public void AddDefaultBuff(float buff)
    {
        defaultBuff += buff;
    }

    public void AddColorMaxBuff(float add)
    {
        colorMaxBonus += add;
    }

    public void AddColorMaxDamageBuff(int add)
    {
        colorMaxDamageBonus += add;
    }

    public int GetColorMaxDamageBuff()
    {
        int damageBonus = 0;
        foreach (ColorSlot slot in colorSlots)
        {
            if (slot.charge == slot.maxCapacity)
            {
                damageBonus += colorMaxDamageBonus;
            }
        }
        return damageBonus;
    }

    /// <summary>
    /// Gets the color buff for the active slots color- Returns 1 if no color buff exists.
    /// </summary>
    /// <returns></returns>
    public float GetColorBuff()
    {
        return GetColorBuff(ActiveSlot().gameColor);
    }

    public void SetRandomBuff()
    {
        float buff = Random.Range(rngMin, rngMax);
        buff *= 100;
        buff = Mathf.Round(buff);
        buff *= 0.01f;
        rngBuff = buff;
        onColorUpdated?.Invoke();
    }

    public void SetRandomBuff(float min, float max)
    {
        rngMax = max;
        rngMin = min;
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
        AddColor(color, amount, null);
    }
    public void AddColor(GameColor color, int amount, ColorSlot fillSlot)
    {
        if(color == null) return;
        if(fillSlot == null) fillSlot = ActiveSlot();

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
        
        if(fillSlot.charge > 0 && !dontMixColor)
            setColor = fillSlot.gameColor.MixColor(color);
        else
            setColor = color;

        fillSlot.AddCharge(amount);
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
        if(ActiveSlot().charge > 0)
        {
            if(playerLight) playerLight.color = colorSlots[activeSlot].gameColor.lightTintColor;
        } else  
        {
            if(playerLight) playerLight.color = Color.white;
        }
    }

    public void MixRandom()
    {
        if (chaosEnabled) AddColor(colorLib.GetRandomPrimaryColor(), 1);
    }

    public void MixRandom(ColorSlot slot)
    {
        if (chaosEnabled) AddColor(colorLib.GetRandomPrimaryColor(), 1, slot);
    }

    public GameColor GetEmptyBottleColor()
    {
        return emptyBottleColor;
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
        float min = 0;
        foreach(float cd in ActiveSlot().storedSpellCDs)
        {
            if (min == 0) min = cd;
            if (min > cd) min = cd;
        }
        ActiveSlot().storedSpellCDs = CreateCDList(newSpell, min);
        onColorSpellChanged?.Invoke(activeSlot);
    }

    public List<float> CreateCDList(ColorSpell spell, float min)
    {
        if (spell == null) spell = defaultSpell;
        int spellCapacity = spell.storedSpells + bonusSpells;
        if (spellCapacity > maxStoredSpells) spellCapacity = maxStoredSpells;
        List<float> list = new List<float>();
        list.Add(min);
        for (int i = 1; i < spellCapacity; i++)
        {
            list.Add(0);
            list = SetCoolDownForIndex(list, i, spell.coolDown);
        }
        return list;
    }

    public List<float> UpdateCDList(ColorSpell spell, List<float> oldList)
    {
        List<float> newList = oldList;
        if (spell == null) spell = defaultSpell;
        int spellCapacity = spell.storedSpells + bonusSpells;
        if (spellCapacity > maxStoredSpells) spellCapacity = maxStoredSpells;
        if (newList.Count == spellCapacity) return newList;
        if (newList.Count < spellCapacity)
        {
            float max = Time.fixedTime;
            foreach (float f in newList)
            {
                if (f > max) max = f;
            }
            while (newList.Count < spellCapacity)
            {
                max += CalculateCD(spell.coolDown);
                newList.Add(max);
            }
        } else
        {
            while (newList.Count > spellCapacity)
            {
                float max = 0;
                foreach(float f in newList)
                {
                    if (f > max) max = f;
                }
                newList.Remove(max);
            }
        }
        return newList;
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
        colorSlots[index].coolDown = 0;
        colorSlots[index].storedSpellCDs = CreateCDList(newSpell, 0);
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

        foreach (ColorSlot slot in colorSlots)
        {
            ValidateCDlist(slot);
        }

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

    #region defense

    public bool CheckIfActiveColorMatches(GameColor color)
    {
        return color == ActiveSlot().gameColor;
    }

    public bool CheckRoutedSheild(GameColor color)
    {
        if (!routedSheild) return false;
        if (color == null) return false;
        foreach (ColorSlot slot in colorSlots)
        {
            if (slot.gameColor == color && slot.charge == slot.maxCapacity)
            {
                if (Random.Range(0, 100) > blockDrainColor) AddColor(color, (int) (slot.charge * -routedSheildCost), slot);
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Unity Actions

    public void WhenDamaged(PlayerStats player, EnemyStats enemy)
    {
        //Add events from certain items or spells to activate when damaged
        foreach (ColorSlot slot in colorSlots)
        {
            ColorSpell spell = slot.colorSpell;
            if (spell == null) spell = defaultSpell;
            if (spell.castWhenDamaged)
            {
                if (spellTracker.ContainsKey(spell.spawnKey))
                {
                    spellTracker[spell.spawnKey]++;
                }
                else
                {
                    spellTracker.Add(spell.spawnKey, 0);
                }
                StartCoroutine(PocketSpecialAttack(spell.spawnKey, spellTracker[spell.spawnKey], slot, spell.staggeredSpell));
            }
        }
        EnableRotation();
    }

    public void SpellImactTrigger()
    {
        foreach (ColorSlot slot in colorSlots)
        {
            ColorSpell spell = slot.colorSpell;
            if (spell == null) spell = defaultSpell;
            if (spell.castOnSpellImpact)
            {
                if (spellTracker.ContainsKey(spell.spawnKey))
                {
                    spellTracker[spell.spawnKey]++;
                }
                else
                {
                    spellTracker.Add(spell.spawnKey, 0);
                }
                StartCoroutine(PocketSpecialAttack(spell.spawnKey, spellTracker[spell.spawnKey], slot, spell.staggeredSpell));
            }
        }
    }

    public IEnumerator PocketSpecialAttack(string spell, int delay, ColorSlot slot, bool staggerd)
    {
        yield return new WaitUntil(() => spellTracker[spell] < delay || delay == 0 || spell.Equals("") || !staggerd);
        GetComponent<PlayerCombatSystem>().DashSpecialAttack(slot);
    }

    public void DashSpells()
    {
        
        foreach (ColorSlot slot in colorSlots)
        {
            ColorSpell spell = slot.colorSpell;
            if (spell == null) spell = defaultSpell;
            if (spell.castOnDash && IsSpellReady(slot))
            {
                if (spellTracker.ContainsKey(spell.spawnKey))
                {
                    spellTracker[spell.spawnKey]++;
                } else
                {
                    spellTracker.Add(spell.spawnKey, 0);
                }
                StartCoroutine(DashSpecialAttack(spell.spawnKey, spellTracker[spell.spawnKey], slot, spell.staggeredSpell));
            }
        }
        EnableRotation();

    }

    public IEnumerator DashSpecialAttack(string spell, int delay, ColorSlot slot, bool staggerd)
    {
        yield return new WaitUntil(() => spellTracker[spell] < delay || delay == 0 || spell.Equals("") || !staggerd);
        GetComponent<PlayerCombatSystem>().DashSpecialAttack(slot);
    }

    public void DoubleJumpSpells()
    {
        foreach (ColorSlot slot in colorSlots)
        {
            ColorSpell spell = slot.colorSpell;
            if (spell == null) spell = defaultSpell;
            if (spell.castOnDoubleJump && IsSpellReady(slot))
            {
                if (spellTracker.ContainsKey(spell.spawnKey))
                {
                    spellTracker[spell.spawnKey]++;
                }
                else
                {
                    spellTracker.Add(spell.spawnKey, 0);
                }
                StartCoroutine(DoubleJumpSpecialAttack(spell.spawnKey, spellTracker[spell.spawnKey], slot, spell.staggeredSpell));
            }
        }
        EnableRotation();
    }

    public IEnumerator DoubleJumpSpecialAttack(string spell, int delay, ColorSlot slot, bool staggerd)
    {
        yield return new WaitUntil(() => spellTracker[spell] < delay || delay == 0 || spell.Equals("") || !staggerd);
        GetComponent<PlayerCombatSystem>().DashSpecialAttack(slot);
    }

    public void QuedSpells(string spell)
    {
        if (spellTracker.ContainsKey(spell))
        {
            spellTracker[spell]--;
            if (spellTracker[spell] < 0) spellTracker.Remove(spell);
        }
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
    public List<float> storedSpellCDs = new List<float>();
    public float coolDown = 0;
    public void Init(Image setImage)
    {
        SetGameColor(gameColor);
        SetCharge(charge);
    }

    public void SetCharge(int set)
    {
        charge = set;
        if(charge > maxCapacity)
        {
            charge = maxCapacity;
            GameManager.instance.tipsManager.DisplayTips("filledBottle");
        }
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