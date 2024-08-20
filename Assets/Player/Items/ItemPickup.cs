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
    [SerializeField] public bool setByhand;
    [SerializeField] Item item;
    [SerializeField] GameObject canvas;
    [SerializeField] TMP_Text cost;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    private Coroutine hoverCoroutine;

    int itemCost;

    void OnEnable()
    {
        if(setByhand)
        {
            SetItem(item, item.itemCost);
        }
    }

    /// <summary>
    /// Sets the item for this spawnpoint
    /// </summary>
    /// <param name="setItem"></param>
    public void SetItem(Item setItem, int itemCost)
    {
        this.item = setItem;
        this.itemCost = itemCost;
                
        descriptionText.text = item.description;
        nameText.text = item.name;
        cost.text = "Cost: " + itemCost;

        spriteRenderer.sprite = item.sprite;

        canvas.SetActive(false);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        hoverCoroutine = StartCoroutine(hoverAnimation());
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
        StopCoroutine(hoverCoroutine);
    }

    /// <summary>
    /// Returns the spawnpoints item;
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return item;
    }

    /// <summary>
    /// Returns the cost of this item
    /// </summary>
    /// <returns></returns>
    public int GetItemCost()
    {
        return itemCost;
    }
    private IEnumerator hoverAnimation()
    {
        while (true)
        {
            spriteRenderer.transform.position = new Vector3(
                spriteRenderer.transform.position.x, 
                spriteRenderer.transform.position.y + Mathf.Sin(Time.time * 2) * 0.003f, 
                spriteRenderer.transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }
}
