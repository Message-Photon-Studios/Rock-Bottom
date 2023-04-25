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
    [SerializeField]TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    SpriteRenderer spriteRenderer;
    ItemInventory inventory;

    public void SetItem(Item setItem)
    {
        this.item = setItem;
                
        descriptionText.text = item.description;
        nameText.text = item.name;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite;

        canvas.SetActive(false);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
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
