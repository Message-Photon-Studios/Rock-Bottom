using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps track of the colors that the player has gathered. 
/// </summary>
public class ColorInventory : MonoBehaviour
{
    int startColorSlots;
    [SerializeField] float colorBuff;
    [SerializeField] List<ColorSlot> colorSlots;
    [SerializeField] int activeSlot;
    [SerializeField] InputActionReference changeRightActions;
    [SerializeField] Image[] images;
    

    void Start()
    {
        startColorSlots = colorSlots.Count;
        for (int i = 0; i < startColorSlots; i++)
        {
            colorSlots[i].Init(images[i]);
        }
    }

    void OnEnable()
    {
        changeRightActions.action.performed += (dir) => {RotateActive((int)dir.ReadValue<float>()); };
    }

    void OnDisable()
    {
        changeRightActions.action.performed -= (dir) => {RotateActive((int)dir.ReadValue<float>()); };
    }

    /// <summary>
    /// Changes the active color slot by rotating in a direction
    /// </summary>
    /// <param name="dir"></param>
    public void RotateActive(int dir)
    {
        activeSlot = (colorSlots.Count+activeSlot+dir)%colorSlots.Count;
        Debug.Log(ActiveSlot().gameColor);
    }

    /// <summary>
    /// Returns the color effect from the active color slot
    /// </summary>
    /// <returns></returns>
    public ColorEffect GetActiveColorEffect()
    {
        if(ActiveSlot().charge > 0)
        {
            ActiveSlot().SetCharge(ActiveSlot().charge -1);
            return ActiveSlot().gameColor.colorEffect;
        }

        return null;
    }

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

        return 0;
    }
    
    /// <summary>
    /// Gets the color buff for the active slots color- Returns 1 if no color buff exists.
    /// </summary>
    /// <returns></returns>
    public float GetColorBuff()
    {
        return GetColorBuff(ActiveSlot().gameColor);
    }

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
    }

    /// <summary>
    /// Removes the color from the active color slot
    /// </summary>
    public void RemoveActiveColor()
    {
        ActiveSlot().RemoveColor();
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
    /// Adds a color slot. Can't add more color slots than the UI has specified
    /// </summary>
    public void AddColorSlot()
    {
        colorSlots.Add(new ColorSlot());
        colorSlots[colorSlots.Count-1].Init(images[colorSlots.Count-1]);
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
    }

    /// <summary>
    /// Removes all color and resets the ammount of slots to the initial state.
    /// </summary>
    public void ResetColorSlots()
    {
        RemoveAllColors();
        while(colorSlots.Count > startColorSlots)
            RemoveColorSlot();
    }
}

/// <summary>
/// Defines the traits of a color slot
/// </summary>
[System.Serializable]
public struct ColorSlot
{
    [SerializeField] public Image image ;
    [SerializeField] float imageScale;
    [SerializeField] public int maxCapacity;
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
        image.color = gameColor.color;
    }

    public void RemoveColor()
    {
        gameColor = null;
        image.color = Color.white;
        SetCharge(0);
    }
}
