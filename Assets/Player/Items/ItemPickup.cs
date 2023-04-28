using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the item spawn points
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] float spawnChance = 1f;
    [SerializeField] bool needsPayment;
    Item item;
    [SerializeField] GameObject canvas;
    [SerializeField] TMP_Text cost;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    /// <summary>
    /// Sets the item for this spawnpoint
    /// </summary>
    /// <param name="setItem"></param>
    public void SetItem(Item setItem)
    {
        this.item = setItem;
                
        descriptionText.text = item.description;
        nameText.text = item.name;
        cost.text = "Cost: " + item.itemCost;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite;

        canvas.SetActive(false);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(!needsPayment)
            {
                inventory.EnablePickUp(this);
                cost.gameObject.SetActive(false);
            } else
            {
                inventory.EnableBuyItem(this);
                cost.gameObject.SetActive(true);
            }

            canvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(!needsPayment)
                inventory.DisablePickUp(this);
            else
                inventory.DisableBuyItem(this);
            canvas.SetActive(false);
            cost.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Is called when this item is picked up
    /// </summary>
    public void PickedUp()
    {
        inventory.AddItem(item);
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Returns the spawnpoints item;
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return item;
    }
}
