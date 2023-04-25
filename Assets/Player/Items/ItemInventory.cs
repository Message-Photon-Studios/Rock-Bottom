using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] int coins;
    [SerializeField] List<ItemEffect> items = new List<ItemEffect>();
    [SerializeField] InputActionReference pickUpAction;
    
    private List<Item> pickUpItems = new List<Item>();

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

    public void AddItem(ItemEffect item)
    {
        items.Add(item);
        item.ActivateEffect();
    }

    public void EnablePickUp(Item item)
    {
        pickUpItems.Add(item);
    }

    public void DisablePickUp (Item item) 
    {
        if(pickUpItems.Contains(item)) pickUpItems.Remove(item);    
    }
}
