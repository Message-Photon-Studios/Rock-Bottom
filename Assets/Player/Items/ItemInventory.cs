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
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] InputActionReference pickUpAction;
    
    private List<ItemPickup> pickUpItems = new List<ItemPickup>();

    /// <summary>
    /// Is called whenever the player picks up an item. Sends the item picked up
    /// </summary>
    public UnityAction<Item> onItemPickedUp;

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

    void OnEnable()
    {
        pickUp = (InputAction.CallbackContext ctx) => {
            while(pickUpItems.Count > 0)
            {
                pickUpItems[0].PickedUp();
            }
        };

        pickUpAction.action.performed += pickUp;
    }

    void OnDisable()
    {
        pickUpAction.action.performed -= pickUp;
    }

    /// <summary>
    /// Adds an item to the inventory and enables it
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        items.Add(item);
        item.EnableItem();
        onItemPickedUp?.Invoke(item);
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
    /// Adds coins to the players inventory
    /// </summary>
    /// <param name="addCoins"></param>
    public void AddCoins(int addCoins)
    {
        coins += addCoins;
        onCoinsChanged?.Invoke(addCoins);
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
}
