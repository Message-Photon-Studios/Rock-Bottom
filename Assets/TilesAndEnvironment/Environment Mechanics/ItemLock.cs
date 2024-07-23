using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
/// <summary>
/// This class checks if the player has a special key item, in that case the lock is opened.
/// </summary>
public abstract class ItemLock : MonoBehaviour
{
    [SerializeField] Item key;
    [SerializeField] bool consumeKeyOnUnlock;
    [SerializeField] GameObject lockedUI;
    [SerializeField] GameObject unlockabelUI;
    [SerializeField] InputActionReference unlockAction;
    Action<InputAction.CallbackContext> unlock;

    ItemInventory itemInventory;

    bool unlockable = false;

    void Start()
    {
        itemInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
    }

    private void OnEnable() {
        unlock = (InputAction.CallbackContext ctx) => {Unlock();};
        unlockAction.action.performed += unlock;
    }
    
    private void OnDisable() 
    {
        unlockAction.action.performed -= unlock;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(itemInventory.HasItemWithName(key.name))
            {
                unlockable = true;
                unlockabelUI.SetActive(true);
                lockedUI.SetActive(false);
            } else
            {
                lockedUI.SetActive(true);
                unlockabelUI.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Player")) 
        {
            lockedUI.SetActive(false);
            unlockabelUI.SetActive(false);
            unlockable = false;
        }
    }

    private void Unlock()
    {
        if(unlockable)
        {
            OpenLock();
            if(consumeKeyOnUnlock)
            {
                itemInventory.RemoveItemWithName(key.name);
            }
            lockedUI.SetActive(false);
            unlockabelUI.SetActive(false);
        }
    }


    protected abstract void OpenLock();
}
