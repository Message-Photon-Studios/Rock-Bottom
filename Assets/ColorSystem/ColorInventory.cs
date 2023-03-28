using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ColorInventory : MonoBehaviour
{
    [SerializeField] List<ColorSlot> colorSlots;
    
    [SerializeField] int activeSlot;
    [SerializeField] InputActionReference changeRightActions;
    

    void Start()
    {
        foreach (ColorSlot slot in colorSlots)
        {
            slot.Init();
        }
    }

    public void RotateActive(int dir)
    {
        activeSlot = (colorSlots.Count+activeSlot+dir)%colorSlots.Count;
    }

    public ColorEffect UseActiveColorEffect()
    {
        if(ActiveSlot().charge > 0)
        {
            ActiveSlot().SetCharge(ActiveSlot().charge -1);
            return ActiveSlot().gameColor.colorEffect;
        }

        return null;
    }

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

    public void RemoveAllColors()
    {
        foreach (ColorSlot item in colorSlots)
        {
            item.RemoveColor();
        }
    }

    public void RemoveActiveColor()
    {
        ActiveSlot().RemoveColor();
    }

    private ColorSlot ActiveSlot()
    {
        return colorSlots[activeSlot];
    }
}

[System.Serializable]
public struct ColorSlot
{
    [SerializeField] public Image image ;
    [SerializeField] float imageScale;
    [SerializeField] public int maxCapacity;
    [SerializeField] public int charge;
    [SerializeField] public GameColor gameColor;

    public void Init()
    {
        imageScale = image.rectTransform.sizeDelta.y;
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
