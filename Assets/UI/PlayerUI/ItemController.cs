using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    ItemInventory inventory;
    [SerializeField] GameObject itemContainer;

    private void OnEnable() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        LoadItems();
    }

    private void OnDisable() {
        
    }

    private void LoadItems() {
        foreach(Item item in inventory.getItems()) {
            GameObject container = new GameObject();
            Image image = container.AddComponent<Image>();
            container.GetComponent<RectTransform>().SetParent(itemContainer.transform);
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            image.SetNativeSize();
            container.SetActive(true);
        }
    }
}
