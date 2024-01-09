using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class SpellPickup : MonoBehaviour
{
    [SerializeField] float spawnChance = 1f;
    [SerializeField] bool needsPayment;
    [SerializeField] ColorSpell colorSpell;
    [SerializeField] GameObject canvas;
    [SerializeField] TMP_Text cost;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    SpriteRenderer spriteRenderer;
    ColorInventory inventory;
    bool pickedup = false;

    /// <summary>
    /// Sets the color spell for this spawnpoint
    /// </summary>
    /// <param name="setItem"></param>
    public void SetSpell(ColorSpell setSpell)
    {
        if (colorSpell == null || pickedup) this.colorSpell = setSpell;
                
        descriptionText.text = colorSpell.description;
        nameText.text = colorSpell.name;
        cost.text = "Cost: " + colorSpell.spellCost;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = colorSpell.GetBottleSprite().smallSprite;

        canvas.SetActive(false);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
    }

    /// <summary>
    /// Randomly destroys the spawn point depending on the initial conditions
    /// </summary>
    public void RandomSpawnDestroy()
    {
        if(spawnChance < 1f)
        {
            float rng = UnityEngine.Random.Range(0,1f);
            if(rng > spawnChance)
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.EnablePickUp(this);
            if(needsPayment)
            {
                cost.gameObject.SetActive(true);
            }

            canvas.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.DisablePickUp(this);
            canvas.SetActive(false);
            cost.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Is called when this color spell is picked up
    /// </summary>
    public void PickedUp()
    {
        pickedup = true;
        ColorSpell tmp = inventory.GetActiveColorSpell();
        inventory.ChangeActiveSlotColorSpell(colorSpell);
        needsPayment = false;
        SetSpell(tmp);
    }

    /// <summary>
    /// Returns the spawnpoints color spell;
    /// </summary>
    /// <returns></returns>
    public ColorSpell GetSpell()
    {
        return colorSpell;
    }

    /// <summary>
    /// Returns true if the spell needs payment
    /// </summary>
    /// <returns></returns>
    public bool GetNeedsPayement()
    {
        return needsPayment;
    }
}
