using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using System.Linq;
/// <summary>
/// This class checks if the player has a special key item, in that case the lock is opened.
/// </summary>
public abstract class ItemLock : MonoBehaviour
{
    [SerializeField] Item key;
    [SerializeField] bool consumeKeyOnUnlock;
    [SerializeField] bool consumePermanentItem;
    [SerializeField] UIMenu lockedUI;
    [SerializeField] UIMenu unlockabelUI;
    [SerializeField] InputActionReference unlockAction;
    [SerializeField] AudioSource audioSource;
    Action<InputAction.CallbackContext> unlock;
    bool unlocked = false;

    ItemInventory itemInventory;

    bool unlockable = false;

    void Start()
    {
        itemInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
    }

    private void OnEnable() {
        unlock = (InputAction.CallbackContext ctx) => {Unlock();};
        unlockAction.action.performed += unlock;

        if(unlocked) SetUnlocked();
    }
    
    private void OnDisable() 
    {
        unlockAction.action.performed -= unlock;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !unlocked)
        {
            if(itemInventory.HasItemWithName(key.name))
            {
                unlockable = true;
                unlockabelUI.OpenMenu();
            } else
            {
                lockedUI.OpenMenu();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Player")) 
        {
            if (unlockable)
            {
                unlockabelUI.CloseMenu();
            }
            else
            {
                lockedUI.CloseMenu();
            }
            unlockable = false;
        }
    }

    private void Unlock()
    {
        if(unlocked)
        {
            SetUnlocked();
            return;
        }

        if(unlockable)
        {
            unlocked = true;
            audioSource.Play();
            OpenLock();
            if(consumeKeyOnUnlock)
            {
                if(consumePermanentItem)
                {
                    List<Item> permItems = PermanentUpgradeManager.instance.upgrades.GetPermanentItems().ToList<Item>();
                    if(permItems.Contains(key))
                    {
                        permItems.Remove(key);
                        PermanentUpgradeManager.instance.upgrades.SetPermanentItems(permItems);
                        GameManager.instance.keepUnlocked =  true;
                        DataPersistenceManager.instance.SaveGame();
                    }
                }

                itemInventory.RemoveItemWithName(key.name);
            }
            unlockabelUI.CloseMenu();
        }
    }

    public void SavedUnlock()
    {
        unlocked = true;
        Unlock();
    }

    /// <summary>
    /// Sets the lock as already being unlocked.
    /// </summary>
    protected abstract void SetUnlocked();

    /// <summary>
    /// Opens the lock as normal.
    /// </summary>
    protected abstract void OpenLock();
}
