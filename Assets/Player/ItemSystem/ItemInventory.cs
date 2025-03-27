using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

/// <summary>
/// Handles the players picked up items and item pickup
/// </summary>
public class ItemInventory : MonoBehaviour
{
    [SerializeField] int coins;
    private int startCoins;
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] CoinPickupEffect coinPickupEffect;
    public float coinBoost {get; private set;} = 1;
    
    /// <summary>
    /// Is called whenever the player picks up an item. Sends the item picked up
    /// </summary>
    public UnityAction<Item> onItemPickedUpOrRemoved;

    /// <summary>
    /// Is called whenever the players cois is changed. Sends an it which indicate the how much the players coins changed with.
    /// </summary>
    public UnityAction<int> onCoinsChanged;


    void Awake()
    {
        GameManager.instance.onPrepareNewRun += SetPermanentItems;
        GameManager.instance.onStartedNewRun += LoadPermanentItems;
        GameManager.instance.onLoadedCaveTown += LoadPermanentItems;
    }
    void Start()
    {
        startCoins = coins;
    }

    void OnDisable()
    {
        GameManager.instance.onPrepareNewRun -= SetPermanentItems;
        GameManager.instance.onStartedNewRun -= LoadPermanentItems;
        GameManager.instance.onLoadedCaveTown -= LoadPermanentItems;
    }

    /// <summary>
    /// Adds an item to the inventory and enables it
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        items.Add(item);
        item.EnableItem();
        onItemPickedUpOrRemoved?.Invoke(item);
    }

    /// <summary>
    /// Adds coins to the players inventory
    /// </summary>
    /// <param name="addCoins"></param>
    public void AddCoins(int addCoins)
    {
        coins += addCoins;
        onCoinsChanged?.Invoke(addCoins);

        CoinPickupEffect coinPickupEffectInstance = Instantiate(coinPickupEffect, transform.position, Quaternion.identity);
        coinPickupEffectInstance.transform.parent = transform;
    }

    /// <summary>
    /// Returns how much coins the player has
    /// </summary>
    /// <returns></returns>
    public int GetCoins()
    {
        return coins;
    }

    /// <summary>
    /// Returns all the items the player has.
    /// </summary>
    /// <returns></returns>
    public List<Item> getItems() {
        return items;
    }

    public int getItemAmoutWithName(string name)
    {
        int count = 0;
        foreach (Item item in getItems())
        {
            if (item.name.Equals(name)) count++;
        }
        return count;
    }

    /// <summary>
    /// Adds a coin boost modifier
    /// </summary>
    /// <param name="boost"></param>
    public void AddCoinBoost(float boost)
    {
        coinBoost += boost;
    }

    /// <summary>
    /// Makes the player pay the specified cost. If the player don't have enough money the method returns false and no
    /// coins are removed form the player. If the payment was successfully done the method returns true.
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool PayCost(int cost)
    {
        if(!HasEnoughCoins(cost)) return false;
        coins -= cost;
        onCoinsChanged?.Invoke(-cost);
        return true;
    }

    /// <summary>
    /// Returns true if the player has enough coins for the cost
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool HasEnoughCoins(int cost)
    {
        return (coins >= cost);
    }

    /// <summary>
    /// Returns true if the inventory contains at least one item with the specified name.
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public bool HasItemWithName(string itemName)
    {
        return items.Exists(item => {return item.name.Equals(itemName);});
    }

    /// <summary>
    /// Removes one item with the specified name.
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public bool RemoveItemWithName(string itemName)
    {
        Item item = items.Find(remove => {return remove.name.Equals(itemName);});
        if(item == null) return false;

        item.DisableItem();
        items.Remove(item);
        onItemPickedUpOrRemoved?.Invoke(item);
        return true;
    }

    public void SetPermanentItems()
    {
        PermanentUpgradeManager.instance.upgrades.SetPermanentItems(items);
    }

    private void LoadPermanentItems()
    {
        items.AddRange(PermanentUpgradeManager.instance.upgrades.GetPermanentItems());
    }
}
