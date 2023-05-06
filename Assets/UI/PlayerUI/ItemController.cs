using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    //Items inventory.
    ItemInventory inventory;

    //GameObject with grid to put items in.
    [SerializeField] GameObject itemsContainer;

    [SerializeField] GameObject selectedItemContainer;
    [SerializeField] Image selectedImage;
    [SerializeField] TMP_Text selectedName;
    [SerializeField] TMP_Text selectedDesc;


    private void OnEnable() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        inventory.onItemPickedUp += AddItem;
        selectedItemContainer.SetActive(false);
    }

    private void OnDisable() {
        
    }

    /// <summary>
    /// Adds the item as an image in the inventory.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    private void AddItem(Item item) {
        GameObject container = new GameObject();
        Image image = container.AddComponent<Image>();
        container.GetComponent<RectTransform>().SetParent(itemsContainer.transform);
        container.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        container.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        image.sprite = item.sprite;
        image.SetNativeSize();
        container.SetActive(true);
        ShowSelectedItem(item);
    }

    private void ShowSelectedItem(Item item) {
        selectedItemContainer.SetActive(true);
        selectedImage.sprite = item.sprite;
        selectedName.text = item.name;
        selectedDesc.text = item.description;
    }
}
