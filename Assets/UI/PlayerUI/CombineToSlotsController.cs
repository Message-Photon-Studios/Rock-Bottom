using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CombineToSlotsController : MonoBehaviour
{
    [SerializeField] TMP_Text focusText;
    [SerializeField] Image focusImage;
    [SerializeField] GameObject[] arrows;
    [SerializeField] float openYpos;
    [SerializeField] RectTransform slotsParent;
    [SerializeField] GameObject uiObj;

    private Vector3 startingPos;
    
    void Start()
    {
        startingPos = slotsParent.position;
        Player.instance.playerCombatSystem.onSpellPickupMode += AddBottle;
        Player.instance.playerCombatSystem.onColorPickupMode += AddColor;
    }

    void OnDestroy()
    {
        Player.instance.playerCombatSystem.onSpellPickupMode -= AddBottle;
        Player.instance.playerCombatSystem.onColorPickupMode -= AddColor;
    }

    public void AddColor(bool pickup, GameColor colorToAdd)
    {
        if(!pickup)
        {
            CloseUi();
            return;
        }

        if(Player.instance.playerCombatSystem.colorWell.GetColorAmount() <= 0 || Player.instance.playerCombatSystem.colorWell.color == null)
        {
            DivideColorShrine(colorToAdd);
            return;
        }

        int slotCount = Player.instance.colorInventory.colorSlots.Count;

        for (int i = 0; i < slotCount; i++)
        {
            arrows[i].GetComponent<Image>().color = Player.instance.colorInventory.GetColorSlotColor(i).MixColor(colorToAdd).plainColor;
        }

        OpenUi(colorToAdd.name, colorToAdd.colorIcon, slotCount);
    }

    public void DivideColorShrine(GameColor colorToShrine)
    {
        int slotCount = Player.instance.colorInventory.colorSlots.Count;
        OpenUi(colorToShrine.name, colorToShrine.colorIcon, slotCount);

        for (int i = 0; i < slotCount; i++)
        {
            if(Player.instance.colorInventory.GetColorSlotColor(i) == null || !Player.instance.colorInventory.GetColorSlotColor(i).SharesRootColor(colorToShrine)) 
            {
                arrows[i].SetActive(false);
                continue;
            }
            arrows[i].GetComponent<Image>().color = colorToShrine.SharedRootMix(Player.instance.colorInventory.GetColorSlotColor(i)).plainColor;
            arrows[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.forward*180);
        }
    }

    public void AddBottle(bool pickup, ColorSpell spellToAdd)
    {
        if(!pickup)
        {
            CloseUi();
            return;
        }

        int slotCount = Player.instance.colorInventory.colorSlots.Count;

        for (int i = 0; i < slotCount; i++)
        {
            arrows[i].GetComponent<Image>().color = Color.white;
        }

        OpenUi(spellToAdd.bottleName.GetLocalizedString(), spellToAdd.GetBottleSprite().smallSprite, slotCount);
    }

    private void OpenUi (string text, Sprite sprite, int slotCount)
    {
        foreach (GameObject obj in arrows)
        {
            obj.SetActive(false);
        }

        for (int i = 0; i < slotCount; i++)
        {
            arrows[i].SetActive(true);
        }

        focusText.text = text;
        focusImage.sprite = sprite;
        
        //slotsParent.position = startingPos + Vector3.up*openYpos;
        uiObj.SetActive(true);
    }

    public void CloseUi()
    {
        uiObj.SetActive(false);
        foreach (GameObject obj in arrows)
        {
            obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
            obj.SetActive(false);
        }
        //slotsParent.position = startingPos;
    }
}
