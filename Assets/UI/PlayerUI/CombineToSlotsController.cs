using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Steamworks;

public class CombineToSlotsController : UIMenu
{
    [SerializeField] TMP_Text focusText;
    [SerializeField] Image focusImage;
    [SerializeField] GameObject[] colorPickupArrows;
    [SerializeField] GameObject[] divideColorArrows;
    [SerializeField] GameObject[] bottlePickupArrows;
    [SerializeField] Image[] mixIcons;
    [SerializeField] float openYpos;
    [SerializeField] RectTransform slotsParent;

    private Vector3 startingPos;

    void Start()
    {
        startingPos = slotsParent.position;
        Player.instance.playerCombatSystem.onSpellPickupMode += AddBottle;
        Player.instance.playerCombatSystem.onColorPickupMode += AddColor;
        CloseUi();
    }

    void OnDestroy()
    {
        Player.instance.playerCombatSystem.onSpellPickupMode -= AddBottle;
        Player.instance.playerCombatSystem.onColorPickupMode -= AddColor;
    }

    public void AddColor(bool pickup, GameColor colorToAdd)
    {
        if (!pickup)
        {
            CloseUi();
            return;
        }

        if (Player.instance.playerCombatSystem.colorWell.GetColorAmount() <= 0 || Player.instance.playerCombatSystem.colorWell.color == null)
        {
            DivideColorShrine(colorToAdd);
            return;
        }

        int slotCount = Player.instance.colorInventory.colorSlots.Count;

        for (int i = 0; i < slotCount && i < colorPickupArrows.Length && i < mixIcons.Length; i++)
        {
            GameColor mixColor = Player.instance.colorInventory.GetColorSlotColor(i).MixColor(colorToAdd);
            // arrows[i].GetComponent<Image>().color = mixColor.plainColor;
            colorPickupArrows[i].transform.GetChild(0).GetComponent<Image>().color = colorToAdd.plainColor;
            mixIcons[i].sprite = mixColor.colorIcon;
            mixIcons[i].gameObject.SetActive(true);
            colorPickupArrows[i].SetActive(true);
        }

        OpenUi(colorToAdd.name, colorToAdd.colorIcon, slotCount);
    }

    public void DivideColorShrine(GameColor colorToShrine)
    {
        int slotCount = Player.instance.colorInventory.colorSlots.Count;
        OpenUi(colorToShrine.name, colorToShrine.colorIcon, slotCount);

        for (int i = 0; i < slotCount && i < divideColorArrows.Length && i < mixIcons.Length; i++)
        {
            if (Player.instance.colorInventory.GetColorSlotColor(i) == null || !Player.instance.colorInventory.GetColorSlotColor(i).SharesRootColor(colorToShrine))
            {
                colorPickupArrows[i].SetActive(false);
                continue;
            }

            GameColor sharedRootMix = colorToShrine.SharedRootMix(Player.instance.colorInventory.GetColorSlotColor(i));
            //arrows[i].GetComponent<Image>().color = sharedRootMix.plainColor;
            divideColorArrows[i].transform.GetChild(0).GetComponent<Image>().color = sharedRootMix.plainColor;
            divideColorArrows[i].SetActive(true);
            mixIcons[i].sprite = Player.instance.colorInventory.GetColorSlotColor(i).ColorSubtraction(sharedRootMix).colorIcon;
            mixIcons[i].gameObject.SetActive(true);
        }
    }

    public void AddBottle(bool pickup, ColorSpell spellToAdd)
    {
        if (!pickup)
        {
            CloseUi();
            return;
        }

        int slotCount = Player.instance.colorInventory.colorSlots.Count;

        for (int i = 0; i < slotCount && i < bottlePickupArrows.Length; i++)
        {
            bottlePickupArrows[i].SetActive(true);
        }

        OpenUi(spellToAdd.bottleName.GetLocalizedString(), spellToAdd.GetBottleSprite().smallSprite, slotCount);
    }

    private void OpenUi(string text, Sprite sprite, int slotCount)
    {
        focusText.text = text;
        focusImage.sprite = sprite;

        //slotsParent.position = startingPos + Vector3.up*openYpos;
        OpenMenu();
    }

    public void CloseUi()
    {
        CloseMenu();
        foreach (GameObject obj in colorPickupArrows)
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            obj.SetActive(false);
        }

        foreach (GameObject obj in divideColorArrows)
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            obj.SetActive(false);
        }

        foreach (GameObject obj in bottlePickupArrows)
        {
            obj.SetActive(false);
        }


        foreach (Image icon in mixIcons)
        {
            icon.gameObject.SetActive(false);
        }
        //slotsParent.position = startingPos;
    }

    protected override void BeforeClosing()
    {
        if(Player.instance.playerCombatSystem.addColorMode) Player.instance.playerCombatSystem.DeactivateAddColorMode();
    }
}
