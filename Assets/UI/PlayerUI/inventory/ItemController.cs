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
    ItemInventory itemInventory;
    [SerializeField] List<SelectedColor> statColorList;

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

    //Amount of items the player has that is over the inventory display limit.
    private int excessCount = 0;

    //Text showing how many more items you have.
    [SerializeField] TMP_Text excessItemsCounter;

    private List<SelectedInventoryItem> items = new List<SelectedInventoryItem>{};


    private void OnEnable() {
        itemInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        itemsContainer.GetComponent<NotifyInventory>().onInventoryOpened += InventoryOpened;

        itemInventory.onItemPickedUp += AddItem;
        foreach(SelectedColor color in statColorList) {
            color.onInventoryColorSelected += ShowSelectedColor;
            color.onColorLoaded += ItemsLoaded;
        }

        selectedItemContainer.SetActive(false);
        excessItemsCounter.gameObject.SetActive(false);
    }

    private void OnDisable() {
        itemInventory.onItemPickedUp -= AddItem;
        foreach(SelectedColor color in statColorList) {
            color.onInventoryColorSelected -= ShowSelectedColor;
            color.onColorLoaded -= ItemsLoaded;
        }
    }

    /// <summary>
    /// When inventory is opened, hide selected item container and deselect any item.
    /// </summary>
    private void InventoryOpened(){
        selectedItemContainer.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        statColorList[0].GetComponent<Selectable>().Select();
    }

    /// <summary>
    /// Called when last item is loaded which then selects it.
    /// </summary>
    private void ItemsLoaded(){
        eventSystem.SetSelectedGameObject(null);
        if(items.Count > 0) {
            items[items.Count -1].GetComponent<Selectable>().Select();
        } else {
            statColorList[0].GetComponent<Selectable>().Select();
        }
    }

    /// <summary>
    /// Adds the item as an image in the inventory.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    private void AddItem(Item item) {
        if(items.Count >= 69) {
            excessCount += 1;
            ShowExcessItems(excessCount);
        } else {
            SelectedInventoryItem newItem = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            newItem.GetComponent<RectTransform>().SetParent(itemsContainer.transform);
            newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            newItem.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            newItem.Setup(item);
            items.Add(newItem);
            newItem.onInventoryItemSelected += ShowSelectedItem;
            newItem.onItemLoaded += ItemsLoaded;
            if(items.Count > 1) {
                items[items.Count-2].onItemLoaded -= ItemsLoaded;
            }
        }
    }

    /// <summary>
    /// Is called when player has more items than the inventory can show.
    /// Displayes extra amount of items as a + followed by the amount of items
    /// in excess the player has. 
    /// </summary>
    /// <param name="excessCount"></param>
    private void ShowExcessItems(int excessCount)
    {
        excessItemsCounter.gameObject.SetActive(true);
        excessItemsCounter.text = "+" + excessCount;
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

    /// <summary>
    /// Sets up the selected image component with given color.
    /// </summary>
    /// <param name="item"></param>
    private void ShowSelectedColor(GameColor color) {
        Debug.Log("Helo");
        selectedItemContainer.SetActive(true);
        selectedImage.sprite =color.colorIcon;
        selectedName.text = color.name;
        selectedDesc.text = color.description;
    }
}
