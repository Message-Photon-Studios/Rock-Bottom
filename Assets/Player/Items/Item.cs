using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Item : MonoBehaviour
{
    [SerializeField] ItemEffect itemEffect;
    SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    void OnEnable()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
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
        inventory.AddItem(itemEffect);
        GameObject.Destroy(gameObject);
    }

}
