using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

    public void AddItem(Item item)
    {
        items.Add(item);
        item.EnableItem();
    }

    public void EnablePickUp(ItemPickup item)
    {
        pickUpItems.Add(item);
    }

    public void DisablePickUp (ItemPickup item) 
    {
        if(pickUpItems.Contains(item)) pickUpItems.Remove(item);    
    }
}
