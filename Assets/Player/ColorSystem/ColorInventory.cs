using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
    [SerializeField] InputActionReference changeRightActions;
    [SerializeField] Image[] images;

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
    
    #endregion

    #region Setup

    void Start()
    {
        startColorSlots = colorSlots.Count;
    }

    void OnEnable()
    {
        changeRightActions.action.performed += (dir) => {RotateActive((int)dir.ReadValue<float>()); };
    }

    void OnDisable()
    {
        changeRightActions.action.performed -= (dir) => {RotateActive((int)dir.ReadValue<float>()); };
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
        Debug.Log(ActiveSlot().gameColor);
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

            onColorUpdated?.Invoke();
            
            GameColor ret = ActiveSlot().gameColor;

            int charge = ActiveSlot().charge - 1;
            ActiveSlot().SetCharge(charge);

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

    #endregion

    #region Color buff

    /// <summary>
    /// Returns the color buff of the specified color. Returns 1 if no color buff exists.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public float GetColorBuff(GameColor color)
    {
        foreach (ColorSlot slot in colorSlots)
        {
            if(slot.gameColor == color && slot.charge == slot.maxCapacity) 
            {
                return colorBuff;
            }
        }

        return 1;
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

    #region Add and remove colors

    /// <summary>
    /// Adds color to the active color slott. Mixes the colors if color already exist there
    /// </summary>
    /// <param name="color"></param>
    /// <param name="amount"></param>
    public void AddColor(GameColor color, int amount)
    {
        GameColor setColor;
        if(ActiveSlot().gameColor != null)
            setColor = ActiveSlot().gameColor.MixColor(color);
        else
            setColor = color;
        int setAmount = ActiveSlot().charge + amount;
        setAmount = (setAmount > ActiveSlot().maxCapacity)?  ActiveSlot().maxCapacity : setAmount;

        colorSlots[activeSlot].SetCharge(setAmount);
        colorSlots[activeSlot].SetGameColor(setColor);

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

    #endregion

    #region Add and remove color slots

    /// <summary>
    /// Adds a color slot. Can't add more color slots than the UI has specified
    /// </summary>
    public void AddColorSlot()
    {
        colorSlots.Add(new ColorSlot());
        onColorSlotsChanged?.Invoke();
    }

    /// <summary>
    /// Removes the last color slot
    /// </summary>
    public void RemoveColorSlot()
    {
        colorSlots.RemoveAt(colorSlots.Count-1);
        if(activeSlot >= colorSlots.Count) 
            activeSlot = colorSlots.Count-1;
        images[colorSlots.Count].transform.parent.gameObject.SetActive(false);
        onColorSlotsChanged?.Invoke();
    }

    /// <summary>
    /// Removes all color and resets the ammount of slots to the initial state.
    /// </summary>
    public void ResetColorSlots()
    {
        RemoveAllColors();
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
    [SerializeField] public Image image ;
    [SerializeField] float imageScale;
    [SerializeField] public int maxCapacity = 6;
    [SerializeField] public int charge;
    [SerializeField] public GameColor gameColor;

    public void Init(Image setImage)
    {
        image = setImage;
        imageScale = image.rectTransform.sizeDelta.y;
        image.transform.parent.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        SetGameColor(gameColor);
        SetCharge(charge);
    }

    public void SetCharge(int set)
    {
        charge = set;
        if(charge <= 0)
        {
            gameColor = null;
            image.color = Color.white;
        }

        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, imageScale*((float)charge/maxCapacity));

    }
    public void SetGameColor(GameColor set) 
    {
        if(set == null) return;
        gameColor = set;
        image.color = gameColor.colorMat.color;
    }

    public void RemoveColor()
    {
        gameColor = null;
        image.color = Color.white;
        SetCharge(0);
    }
}

# endregion