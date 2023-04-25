using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] float spawnChance = 1f;
    Item item;
    [SerializeField] GameObject canvas;
    [SerializeField] TMP_Text text;
    SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    void OnEnable()
    {
        canvas.SetActive(false);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        if(spawnChance < 1f)
        {
            float rng = UnityEngine.Random.Range(0,spawnChance);
            if(rng > spawnChance)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void SetItem(Item setItem)
    {
        this.item = setItem;
                
        text.text = item.description;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.EnablePickUp(this);
            canvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.DisablePickUp(this);
            canvas.SetActive(false);
        }
    }

    public void PickedUp()
    {
        inventory.AddItem(item);
        GameObject.Destroy(gameObject);
    }

}
