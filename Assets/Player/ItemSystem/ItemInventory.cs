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
    [SerializeField] InputActionReference pickUpAction;
    [SerializeField] CoinPickupEffect coinPickupEffect;
    public float coinBoost {get; private set;} = 1;
    
    private List<ItemPickup> pickUpItems = new List<ItemPickup>();

    private List<ItemPickup> buyItems = new List<ItemPickup>();

    private Inspired inspired;

    /// <summary>
    /// Is called whenever the player picks up an item. Sends the item picked up
    /// </summary>
    public UnityAction<Item> onItemPickedUpOrRemoved;

    /// <summary>
    /// Is called whenever an item gets in or gets out of range of being picked up.
    /// Sends a bool that indicates if the item got in ragne (bool == true) or the item got out of range (bool == false)
    /// of being picked up.
    /// </summary>
    public UnityAction<bool> onItemInRange;

    /// <summary>
    /// Is called whenever the players cois is changed. Sends an it which indicate the how much the players coins changed with.
    /// </summary>
    public UnityAction<int> onCoinsChanged;

    Action<InputAction.CallbackContext> pickUp;


    void Awake()
    {
        GameManager.instance.onPrepareNewRun += SetPermanentItems;
        GameManager.instance.onStartedNewRun += LoadPermanentItems;
        GameManager.instance.onLoadedCaveTown += LoadPermanentItems;
    }
    void Start()
    {
        startCoins = coins;
        pickUp = (InputAction.CallbackContext ctx) => {
            while(pickUpItems.Count > 0)
            {
                pickUpItems[0].PickedUp();
            }

            while (buyItems.Count > 0)
            {
                if(PayCost(buyItems[0].GetItemCost()))
                {
                    buyItems[0].PickedUp();
                } else buyItems.RemoveAt(0);
            }

            if(inspired) 
            {
                if(GameManager.instance.TryRemovePetrifiedPigment(inspired.petrifiedPigmentCost))
                    inspired.TriggerUnlock();
            }
        };

        pickUpAction.action.performed += pickUp;
    }

    void OnDisable()
    {
        pickUpAction.action.performed -= pickUp;
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
    /// Enables this item to be picked up whenever a pickup event happens
    /// </summary>
    /// <param name="item"></param>
    public void EnablePickUp(ItemPickup item)
    {
        pickUpItems.Add(item);
        onItemInRange?.Invoke(true);
    }

    /// <summary>
    /// Disables this item from being picked up whenever a pickup event happens
    /// </summary>
    /// <param name="item"></param>
    public void DisablePickUp (ItemPickup item) 
    {
        if(!pickUpItems.Contains(item)) return;
        pickUpItems.Remove(item);    
        onItemInRange?.Invoke(false);
    }
    
    /// <summary>
    /// Enabels an item to be bought
    /// </summary>
    /// <param name="item"></param>
    public void EnableBuyItem(ItemPickup item)
    {
        buyItems.Add(item);
        onItemInRange?.Invoke(true);
    }

    /// <summary>
    /// Disables an item from being bought
    /// </summary>
    /// <param name="item"></param>
    public void DisableBuyItem(ItemPickup item)
    {
        if(!buyItems.Contains(item)) return;
        buyItems.Remove(item);
        onItemInRange?.Invoke(false);
    }

    public void EnableInspired(Inspired inspired)
    {
        this.inspired = inspired;
    }

    public void DisableInspired()
    {
        this.inspired = null;
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
        if(cost > coins) return false;
        coins -= cost;
        onCoinsChanged?.Invoke(-cost);
        return true;
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
