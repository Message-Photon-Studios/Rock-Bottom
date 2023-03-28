using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorInventory : MonoBehaviour
{
    [SerializeField] List<ColorSlot> colorSlots;
    
    [SerializeField] int activeSlot;

    void Start()
    {
        foreach (ColorSlot slot in colorSlots)
        {
            slot.SetGameColor(slot.gameColor);
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

        GameColor setColor = ActiveSlot().gameColor.MixColor(color);
        int setAmount = ActiveSlot().charge + amount;
        setAmount = (setAmount > ActiveSlot().maxCapacity)?  ActiveSlot().maxCapacity : setAmount;

        colorSlots[activeSlot].SetCharge(setAmount);
        colorSlots[activeSlot].SetGameColor(setColor);
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
    [SerializeField] public int maxCapacity;
    [SerializeField] public int charge;
    [SerializeField] public GameColor gameColor;

    public void SetCharge(int set)
    {
        charge = set;
        if(charge <= 0)
        {
            image.color = Color.white;
        }
    }
    public void SetGameColor(GameColor set) 
    {
        gameColor = set;
        if(gameColor != null)
            image.color = gameColor.color;
        else
            image.color = Color.white;
    }
}
