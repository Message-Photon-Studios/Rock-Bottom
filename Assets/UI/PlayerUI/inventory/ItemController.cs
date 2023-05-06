using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class ItemController : MonoBehaviour
{
    //Items inventory.
    ItemInventory inventory;

    //GameObject with grid to put items in.
    [SerializeField] GameObject itemsContainer;

    //Prefab for UI component of each item in the inventory.
    [SerializeField] SelectedInventoryItem prefab;

    //Containers that holds the selected item components.
    [SerializeField] GameObject selectedItemContainer;
    [SerializeField] Image selectedImage;
    [SerializeField] TMP_Text selectedName;
    [SerializeField] TMP_Text selectedDesc;

    //Event system used in inventory.
    [SerializeField] EventSystem eventSystem;


    private void OnEnable() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        itemsContainer.GetComponent<NotifyInventory>().onInventoryOpened += InventoryOpened;
        inventory.onItemPickedUp += AddItem;
        selectedItemContainer.SetActive(false);
    }

    /// <summary>
    /// When inventory is opened, hide selected item container and deselect any item.
    /// </summary>
    private void InventoryOpened(){
        selectedItemContainer.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Adds the item as an image in the inventory.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    private void AddItem(Item item) {
        SelectedInventoryItem newItem = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        newItem.GetComponent<RectTransform>().SetParent(itemsContainer.transform);
        newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        newItem.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        newItem.Setup(item);
        newItem.onInventoryItemSelected += ShowSelectedItem;
    }

    /// <summary>
    /// Sets up the selected image component with given item.
    /// </summary>
    /// <param name="item"></param>
    private void ShowSelectedItem(Item item) {
        selectedItemContainer.SetActive(true);
        selectedImage.sprite = item.sprite;
        selectedName.text = item.name;
        selectedDesc.text = item.description;
    }
}
