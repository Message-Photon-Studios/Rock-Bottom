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

    //Color inventory.
    ColorInventory colorInventory;

    //List with all color stats components.
    [SerializeField] List<SelectedColor> statColorList;

    //GameObject with grid to put items in.
    [SerializeField] GameObject itemsContainer;

    //itemPrefab for UI component of each item in the inventory.
    [SerializeField] SelectedInventoryItem itemPrefab;

    //bottlePrefab for UI component of each bottle in the inventory.
    [SerializeField] SelectedBottle bottlePrefab;

    //Container that holds the selected bottles.
    [SerializeField] GameObject bottlesContainer;

    //Containers that holds the selected item components.
    [SerializeField] GameObject selectedItemContainer;
    [SerializeField] Image selectedImage;
    [SerializeField] TMP_Text selectedName;
    [SerializeField] TMP_Text selectedDesc;
    [SerializeField] int descriptionFontSize;

    //Event system used in inventory.
    [SerializeField] EventSystem eventSystem;

    //Amount of items the player has that is over the inventory display limit.
    private int excessCount = 0;

    //Text showing how many more items you have.
    [SerializeField] TMP_Text excessItemsCounter;

    //Holds all item prefabs.
    private List<SelectedInventoryItem> items = new List<SelectedInventoryItem>{};

    //Holds all bottle prefabs.
    private List<SelectedBottle> bottles = new List<SelectedBottle>{};


    private void OnEnable() {
        itemInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        itemsContainer.GetComponent<NotifyInventory>().onInventoryOpened += InventoryOpened;
        colorInventory = colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();

        colorInventory.onColorSlotsChanged += BottleAmountChanged;
        colorInventory.onColorSpellChanged += BottleChanged;
        foreach(SelectedColor color in statColorList) {
            color.onInventoryColorSelected += ShowSelectedColor;
            color.onColorLoaded += ItemsLoaded;
        }

        for(int i = 0; i < colorInventory.colorSlots.Count; i++) {
            AddBottle(colorInventory.GetColorSpell(i));
        }
        BottleNavigation();

        selectedDesc.fontSize = descriptionFontSize;

        selectedItemContainer.SetActive(false);
        excessItemsCounter.gameObject.SetActive(false);
    }

    private void OnDisable() {
        colorInventory.onColorSpellChanged -= BottleChanged;
        colorInventory.onColorSlotsChanged -= BottleAmountChanged;
        foreach(SelectedColor color in statColorList) {
            color.onInventoryColorSelected -= ShowSelectedColor;
            color.onColorLoaded -= ItemsLoaded;
        }
    }

    /// <summary>
    /// When inventory is opened, hide selected item container and deselect any item.
    /// </summary>
    private void InventoryOpened(){
        items = new List<SelectedInventoryItem>();
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < itemsContainer.transform.childCount; i++)
        {
            objs.Add(itemsContainer.transform.GetChild(i).gameObject);
        }
        itemsContainer.transform.DetachChildren();
        for (int i = 0; i < objs.Count; i++)
        {
            Destroy(objs[i]);
        }
        
        foreach(Item item in itemInventory.getItems())
        {
            AddItem(item);
        }

        selectedItemContainer.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        statColorList[0].GetComponent<Selectable>().Select();
    }

    /// <summary>
    /// When a bottle is added, create UI prefab and set up new navigation.
    /// </summary>
    private void BottleAmountChanged() {
        AddBottle(colorInventory.GetColorSpell(colorInventory.colorSlots.Count -1));
        BottleNavigation();
    }

    /// <summary>
    /// If bottle changed, update information with new bottle from index.
    /// </summary>
    /// <param name="index"></param>
    private void BottleChanged(int index) {
        bottles[index].Setup(colorInventory.GetColorSpell(index));
    }

    /// <summary>
    /// Adds a bottle to the inventory acording to colorspell that was given.
    /// </summary>
    /// <param name="bottle"></param>
    private void AddBottle(ColorSpell bottle) {
        SelectedBottle newBottle = Instantiate(bottlePrefab, new Vector3(0,0,0), Quaternion.identity);
        newBottle.GetComponent<RectTransform>().SetParent(bottlesContainer.transform);
        newBottle.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 110);
        newBottle.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        newBottle.Setup(bottle);
        bottles.Add(newBottle);
        newBottle.onInventoryBottleSelected += ShowSelectedBottle;
    }

    /// <summary>
    /// Sets up navigation for all bottles and other related components with current bottles.
    /// </summary>
    private void BottleNavigation() {
        Navigation redColorNav = statColorList[0].GetComponent<Selectable>().navigation;
        redColorNav.mode = Navigation.Mode.Explicit;
        redColorNav.selectOnLeft = null;
        redColorNav.selectOnDown = statColorList[1].GetComponent<Selectable>();
        redColorNav.selectOnUp = bottles[bottles.Count/2].GetComponent<Selectable>();
        if(items.Count == 0) {
            redColorNav.selectOnRight = null;
        } 
        statColorList[0].GetComponent<Selectable>().navigation = redColorNav;

        Navigation rainbowColorNav = statColorList[6].GetComponent<Selectable>().navigation;
        rainbowColorNav.selectOnDown = bottles[bottles.Count/2].GetComponent<Selectable>();
        statColorList[6].GetComponent<Selectable>().navigation = rainbowColorNav;

        for(int i = 0; i < bottles.Count; i++) {
            Navigation nav = bottles[i].GetComponent<Selectable>().navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = statColorList[6].GetComponent<Selectable>();
            nav.selectOnDown = statColorList[0].GetComponent<Selectable>();

            if(i == 0) {
                nav.selectOnLeft = bottles[bottles.Count-1].GetComponent<Selectable>();
            } else {
                nav.selectOnLeft = bottles[i-1].GetComponent<Selectable>();
            }

            if(i == (bottles.Count-1)) {
                 nav.selectOnRight = bottles[0].GetComponent<Selectable>();
            } else {
                nav.selectOnRight = bottles[i+1].GetComponent<Selectable>();
            }

            bottles[i].GetComponent<Selectable>().navigation = nav;
        }
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
            SelectedInventoryItem newItem = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newItem.GetComponent<RectTransform>().SetParent(itemsContainer.transform);
            newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            newItem.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            newItem.Setup(item);
            items.Add(newItem);
            newItem.onInventoryItemSelected += ShowSelectedItem;
            newItem.onItemLoaded += ItemsLoaded;
            if(items.Count > 1) {
                items[items.Count-2].onItemLoaded -= ItemsLoaded;
                NavigationSetup(newItem);
            } else if(items.Count == 1) {
                Navigation itemNav = newItem.GetComponent<Selectable>().navigation;
                itemNav.mode = Navigation.Mode.Explicit;
                itemNav.selectOnLeft = statColorList[0].GetComponent<Selectable>();
                itemNav.selectOnRight = null;
                itemNav.selectOnDown = null;
                itemNav.selectOnUp = null;
                newItem.GetComponent<Selectable>().navigation = itemNav;

                foreach (SelectedColor colorInfo in statColorList) {
                    Navigation nav = colorInfo.GetComponent<Selectable>().navigation;
                    nav.selectOnRight = newItem.GetComponent<Selectable>();
                    colorInfo.GetComponent<Selectable>().navigation = nav;
                }
            }
            
        }
    }

    /// <summary>
    /// Given an item, sets up navigation for keyboard only/controller.
    /// </summary>
    /// <param name="newItem"></param>
    private void NavigationSetup(SelectedInventoryItem newItem) {
        int nr = items.Count - 1;
            Navigation nav = newItem.GetComponent<Selectable>().navigation;
            Navigation lastNav =  items[nr-1].GetComponent<Selectable>().navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = items[nr-1].GetComponent<Selectable>();
            if((nr%10) != 0) {
            lastNav.selectOnRight = newItem.GetComponent<Selectable>();
            }

            if((nr%10) == 0) {
                nav.selectOnLeft = statColorList[0].GetComponent<Selectable>();
                nav.selectOnRight = null;
            } 
            else if((nr%10) == 9) {
                nav.selectOnRight = items[nr-9].GetComponent<Selectable>();
                nav.selectOnLeft = items[nr-1].GetComponent<Selectable>();
                lastNav.selectOnRight = newItem.GetComponent<Selectable>();
            } else if(nr == 68) {
                nav.selectOnRight = items[nr-8].GetComponent<Selectable>();
                nav.selectOnLeft = items[nr-1].GetComponent<Selectable>();
                lastNav.selectOnRight = newItem.GetComponent<Selectable>();
            }

            if(nr < 10) {
                nav.selectOnUp = null;
                nav.selectOnDown = null;
            } else if (nr > 60) {
                nav.selectOnDown = null;
                nav.selectOnUp = items[nr-10].GetComponent<Selectable>();
                Navigation above = items[nr-10].GetComponent<Selectable>().navigation;
                above.selectOnDown = newItem.GetComponent<Selectable>();
                items[nr-10].GetComponent<Selectable>().navigation = above;
            } else {
                nav.selectOnUp = items[nr-10].GetComponent<Selectable>();
                nav.selectOnDown = null;
                Navigation above = items[nr-10].GetComponent<Selectable>().navigation;
                above.selectOnDown = newItem.GetComponent<Selectable>();
                items[nr-10].GetComponent<Selectable>().navigation = above;
            }

            newItem.GetComponent<Selectable>().navigation = nav;
            items[nr-1].GetComponent<Selectable>().navigation = lastNav;
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
        selectedName.text = item.GetName();
        selectedDesc.text = item.GetDesc();
    }

    /// <summary>
    /// Sets up the selected image component with given color.
    /// </summary>
    /// <param name="item"></param>
    private void ShowSelectedColor(GameColor color) {
        selectedItemContainer.SetActive(true);
        selectedImage.sprite =color.colorIcon;
        selectedName.text = color.name;
        selectedDesc.text = color.description;
    }

    /// <summary>
    /// Sets up the selected image component with given color.
    /// </summary>
    /// <param name="item"></param>
    private void ShowSelectedBottle(ColorSpell bottle) {
        selectedItemContainer.SetActive(true);
        selectedImage.sprite =bottle.GetBottleSprite().bigSprite;
        selectedName.text = bottle.name;
        selectedDesc.text = bottle.description.GetLocalizedString();
    }
}
