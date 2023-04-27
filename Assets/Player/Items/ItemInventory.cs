using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Handles the players picked up items and item pickup
/// </summary>
public class ItemInventory : MonoBehaviour
{
    [SerializeField] int coins;
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] InputActionReference pickUpAction;
    
    private List<ItemPickup> pickUpItems = new List<ItemPickup>();

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
    }

    /// <summary>
    /// Enables this item to be picked up whenever a pickup event happens
    /// </summary>
    /// <param name="item"></param>
    public void EnablePickUp(ItemPickup item)
    {
        pickUpItems.Add(item);
    }

    /// <summary>
    /// Disables this item from being picked up whenever a pickup event happens
    /// </summary>
    /// <param name="item"></param>
    public void DisablePickUp (ItemPickup item) 
    {
        if(pickUpItems.Contains(item)) pickUpItems.Remove(item);    
    }
}
