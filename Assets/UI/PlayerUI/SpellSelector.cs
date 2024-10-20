using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpellSelector : MonoBehaviour
{

    [SerializeField] EventSystem eventSystem;
    [SerializeField] UIController uiController;
    [SerializeField] List<RectTransform> bottles;

    [SerializeField] InputActionReference left;
    [SerializeField] InputActionReference right;

    int currentlySelected = 0;
    ColorInventory colorInv;
    List<ColorSlot> colorSlots;


    // Start is called before the first frame update
    private void OnEnable()
    {
        left.action.performed += rotateLeft;
        right.action.performed += rotateRight;

        colorInv = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        colorSlots = colorInv.colorSlots;
        eventSystem.SetSelectedGameObject(null);
        bottles[colorInv.activeSlot].GetComponent<Selectable>().Select();
        currentlySelected = colorInv.activeSlot;


        var materials = Resources.LoadAll<Material>("Bottles/Materials/Body");
        var capMaterials = Resources.LoadAll<Material>("Bottles/Materials/Cap");
        for (int i = 0; i < bottles.Count; i++)
        {
            Image frameImage = bottles[i].GetChild(0).GetChild(0).GetComponent<Image>();
            frameImage.material = materials[i];
            ColorSlot slot = colorSlots[i];
            frameImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.plainColor : colorInv.defaultColor.GetColor("_Color"));
            frameImage.material.SetFloat("_fill", slot.charge / (float)slot.maxCapacity);

            Image capImage = bottles[i].GetChild(0).GetChild(1).GetComponent<Image>();
            capImage.material = capMaterials[i];
            capImage.material.SetColor("_Color", slot.gameColor != null ? slot.gameColor.colorMat.color : colorInv.defaultColor.GetColor("_Color"));
            capImage.material.SetColor("_PlainColor", slot.gameColor != null ? slot.gameColor.plainColor : colorInv.defaultColor.GetColor("_Color"));
            capImage.material.SetFloat("_Alpha", 0);
            capImage.material.SetFloat("_BloomPower", 0);

            Image capEffect = bottles[i].GetChild(0).GetChild(2).GetComponent<Image>();
            //capEffect.sprite = FullBottleEffectSprites[0];
            capEffect.gameObject.SetActive(false);

            //activeCoroutines.Add(null);
            //BottleChanged(i);

            Image bottle = bottles[i].GetChild(0).GetComponent<Image>();
            Image bottleMask = bottles[i].GetChild(0).GetChild(0).GetComponent<Image>();
            Image capMask = bottles[i].GetChild(0).GetChild(1).GetComponent<Image>();
            BottleSprite bottleSprite = colorInv.GetColorSpell(i).GetBottleSprite();
            bottle.sprite = bottleSprite.bigSprite;
            bottleMask.sprite = bottleSprite.bigSpriteMask;
            capMask.sprite = bottleSprite.bigSpriteCapMask;
        }

    }

    private void OnDisable()
    {
        left.action.performed -= rotateLeft;
        right.action.performed -= rotateRight;

        RectTransform rect = eventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
        int index = bottles.FindIndex(a => a == rect);
        SelectSlot(index - colorInv.activeSlot);
    }

    public void SelectSlot(int i)
    {
        colorInv.RotateActive(i);
    }

    private void rotateLeft(InputAction.CallbackContext ctx) { rotateLeft(); }
    private void rotateLeft()
    {
        currentlySelected--;
        currentlySelected = currentlySelected % bottles.Count;
        bottles[currentlySelected].GetComponent<Selectable>().Select();
    }

    private void rotateRight(InputAction.CallbackContext ctx) { rotateRight(); }
    private void rotateRight()
    {
        currentlySelected++;
        currentlySelected = currentlySelected % bottles.Count;
        bottles[currentlySelected].GetComponent<Selectable>().Select();
    }
}
