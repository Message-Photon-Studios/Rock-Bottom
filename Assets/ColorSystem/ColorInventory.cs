using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ColorInventory : MonoBehaviour
{
    [SerializeField] int startColorSlots;
    [SerializeField] List<ColorSlot> colorSlots;
    
    [SerializeField] int activeSlot;
    [SerializeField] InputActionReference changeRightActions;
    [SerializeField] Image[] images;
    

    void Start()
    {
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

    public void RotateActive(int dir)
    {
        activeSlot = (colorSlots.Count+activeSlot+dir)%colorSlots.Count;
        Debug.Log(ActiveSlot().gameColor);
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

    public void AddColorSlot()
    {
        colorSlots.Add(new ColorSlot());
        colorSlots[colorSlots.Count-1].Init(images[colorSlots.Count-1]);
    }

    public void RemoveColorSlot()
    {
        colorSlots.RemoveAt(colorSlots.Count-1);
        if(activeSlot >= colorSlots.Count) 
            activeSlot = colorSlots.Count-1;
        images[colorSlots.Count].transform.parent.gameObject.SetActive(false);
    }

    public void ResetColorSlots()
    {
        
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
