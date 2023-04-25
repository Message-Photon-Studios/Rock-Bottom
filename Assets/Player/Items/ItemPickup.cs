using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] Item item;
    SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    void OnEnable()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.EnablePickUp(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.DisablePickUp(this);
        }
    }

    public void PickedUp()
    {
        inventory.AddItem(item);
        GameObject.Destroy(gameObject);
    }

}
