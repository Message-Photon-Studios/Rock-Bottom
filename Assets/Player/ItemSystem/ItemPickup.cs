using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

/// <summary>
/// Handles the item spawn points
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : InteractionObject
{
    [Header("Item settings")]
    [SerializeField] public SpawnPointChance spawnChance = SpawnPointChance.LowChance;
    [SerializeField] public bool needsPayment;
    [SerializeField] ItemRarity dropPointRarity;
    [SerializeField] ItemCategory[] availableCategories;
    [SerializeField] public bool allowsHealthItem = true;
    [SerializeField] public bool setByhand;
    [SerializeField] Item item;
    [SerializeField] EnemyStats spawnFromEnemy;

    [SerializeField] public bool enableTizoTrade = true;

    [Header("Functional")]
    [SerializeField] PickUpCanvasController pickUpController;
    [SerializeField] SpriteRenderer spriteRenderer;

  
    ItemInventory inventory;

    private Coroutine hoverCoroutine;

    int itemCost;
    
    protected override void Start()
    {
        base.Start();

        inventory = Player.instance.playerInventory;
        if(spawnFromEnemy != null)
        {
            spawnFromEnemy.onEnemyDeath += SpawnFromEnemy;
        } 
        else if(setByhand && item != null)
        {
            SetItem(item, item.itemCost);
        } else
        {
            Item randomItem = GameManager.instance.itemLibrary.GetRandomItem(dropPointRarity, availableCategories);
            if(randomItem == null) 
            {
                Debug.LogWarning("Item pickup of name " + name + " could not get a random item");
                gameObject.SetActive(false);
            }
            else
                SetItem(randomItem, randomItem.itemCost);
        }
    }

    private void SpawnFromEnemy(EnemyStats deadEnemy)
    {
        transform.position = spawnFromEnemy.transform.position;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down,100f, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlatforms);
        transform.position = hit.point + Vector2.up*.6f;
        SetItem(item, item.itemCost);
        gameObject.SetActive(true);
        spawnFromEnemy.onEnemyDeath -= SpawnFromEnemy;
    }

    void OnDisable()
    {
        if(spawnFromEnemy != null) spawnFromEnemy.onEnemyDeath -= SpawnFromEnemy;
    }

    /// <summary>
    /// Sets the item for this spawnpoint
    /// </summary>
    /// <param name="setItem"></param>
    public void SetItem(Item setItem, int itemCost)
    {
        if(ItemSpellManager.instance == null)
        {
            Debug.LogWarning("Item spell manager not initiated.");
            return;
        }

        if(!setItem.CanBeSpawned())
        {
            setItem = ItemSpellManager.instance.healthItem;
        }

        ItemSpellManager.instance.AddSpawnedItem(setItem);

        this.item = setItem;
        this.itemCost = Mathf.RoundToInt(itemCost*ItemSpellManager.instance.stageCostMultiplier*GameManager.instance.rerunNum);

        spriteRenderer.sprite = item.sprite;
        hoverCoroutine = StartCoroutine(hoverAnimation());
    }

    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);

        if(isClose)
        {
            pickUpController.SetItem(this);
        } else pickUpController.CloseUi();
    }

    protected override void PlayerInteract()
    {
        if(!needsPayment || inventory.PayCost(itemCost))
        {
            pickUpController.CloseUi();
            inventory.AddItem(item);
            GameObject.Destroy(gameObject);
            StopCoroutine(hoverCoroutine);
        }
    }

    /// <summary>
    /// Returns the spawnpoints item;
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return item;
    }

    public bool GetNeedsPayment()
    {
        return needsPayment;
    }

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

public enum SpawnPointChance
{
    Guaranteed,
    HighChance,
    LowChance
}
