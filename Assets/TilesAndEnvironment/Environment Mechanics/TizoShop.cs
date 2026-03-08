using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using System.Diagnostics.Tracing;

public class TizoShop : BigMenu
{   
    [Header("Ui")]
    [SerializeField] TizoTradeModule[] tizoTradeModules;
    [SerializeField] TizoInventoryItem[] inventoryItemImages;
    [SerializeField] GameObject[] infoCards;
    [SerializeField] GameObject infoSeparator;
    [SerializeField] LocalizedString missingString;

    [Header("Shop Settings")]
    [SerializeField] int commonTradeAmount = 5;
    [SerializeField] int rareTradeAmount = 4;
    [SerializeField] int mysticTradeAmount = 1;

    [Header("Trade Blueprints")]
    [SerializeField] TizoTradeBlueprint[] guaranteedBlueprints;
    [SerializeField] TizoTradeBlueprint[] commonTradeBlueprints;
    [SerializeField] TizoTradeBlueprint[] rareTradeBlueprints;
    [SerializeField] TizoTradeBlueprint[] mysticTradeBlueprints;

    List<TizoTrade> trades = new List<TizoTrade>();

    List<Item> availableCostItems = new List<Item>();

    bool shopOpen = false;

    #region Setup
    public void ShopInteract()
    {
        ShopInteract(!shopOpen);
    }

    public void ShopInteract(bool openShop)
    {
        if(!shopOpen && openShop)
        {
            shopOpen = true;
            UpdateUi();
            OpenMenu();
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);

            foreach (GameObject infoCard in infoCards)
            {
                infoCard.SetActive(false);
            }

            for (int i = 0; i < trades.Count && i < tizoTradeModules.Length; i++)
            {
                if(tizoTradeModules[i].hasBought) continue;
                tizoTradeModules[i].GetComponent<Selectable>().Select();
                UpdateInfoCards(trades[i]);
                Mouse.current.WarpCursorPosition(tizoTradeModules[i].transform.position);
                break;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        } else if (shopOpen && !openShop)
        {
            BeforeClosing();
            CloseMenu();
        }
    }

    protected override void BeforeClosing()
    {
        shopOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void LevelLoaded()
    {
        shopOpen = false;
        CloseMenu();
        trades = new List<TizoTrade>();
        SetAvailableCostItems();
        SetTrades();
        SetupUi();
    }

    void SetAvailableCostItems()
    {
        availableCostItems = new List<Item>();
        availableCostItems.AddRange(Player.instance.playerInventory.getItems());

        foreach (ItemPickup itemPickup in FindObjectsOfType<ItemPickup>())
        {
            if(itemPickup.enableTizoTrade && itemPickup.GetItem() != null) availableCostItems.Add(itemPickup.GetItem()); 
        }
    }

    void SetTrades()
    {
        for (int i = 0; i < guaranteedBlueprints.Length; i++)
        {
            TizoTrade trade = CreateTrade(guaranteedBlueprints[i], ItemRarity.Common);
            if (trade != null) trades.Add(trade);
        }
        for (int i = 0; i < commonTradeAmount; i++)
        {
            TizoTrade trade = CreateTrade(commonTradeBlueprints[Random.Range(0, commonTradeBlueprints.Length)], ItemRarity.Common);
            if (trade != null) trades.Add(trade);
        }

        for (int i = 0; i < rareTradeAmount; i++)
        {
            TizoTrade trade = CreateTrade(rareTradeBlueprints[Random.Range(0, rareTradeBlueprints.Length)], ItemRarity.Rare);
            if(trade != null) trades.Add(trade);
        }

        for (int i = 0; i < mysticTradeAmount; i++)
        {
            TizoTrade trade = CreateTrade(mysticTradeBlueprints[Random.Range(0, mysticTradeBlueprints.Length)], ItemRarity.Mythic);
            if(trade != null) trades.Add(trade);
        }
    }

    TizoTrade CreateTrade(TizoTradeBlueprint blueprint, ItemRarity rarity)
    {
        Item[] getItems = new Item[blueprint.getAmount + blueprint.guaranteedItems.Length];

        blueprint.guaranteedItems.CopyTo(getItems, 0);
        int failedTries = 0;
        for (int i = blueprint.guaranteedItems.Length; i < getItems.Length; i++)
        {
            Item getItem = GetTradeItem(blueprint.getRarity[Random.Range(0, blueprint.getRarity.Length)], blueprint.getCategory);
            if (getItem == null || getItems.Contains(getItem))
            {
                if (failedTries > 20)
                {
                    return null;
                }
                failedTries++;
                i--;
                continue;
            }

            else getItems[i] = getItem;
        }

        List<Item> acceptedCostItems = GetAcceptedCostItems(blueprint.costRarity.ToList(), blueprint.costCategory.ToList(), getItems.ToList());

        Item[] costItems = new Item[blueprint.costAmount];
        for (int i = 0; i < blueprint.costAmount; i++)
        {
            costItems[i] = GetCostItem(acceptedCostItems);
            if(costItems[i] == null) return null; 
        }

        for (int i = 0; i < getItems.Length; i++)
        {
            ItemSpellManager.instance.AddSpawnedItem(getItems[i]);
        }
        
        return new TizoTrade(rarity, costItems, getItems);
    }

    List<Item> GetAcceptedCostItems(List<ItemRarity> rarity, List<ItemCategory>category, List<Item> excludeItems)
    {
        List<Item> acceptedCostItems = new List<Item>();
        for (int i = 0; i < availableCostItems.Count; i++)
        {
            if(acceptedCostItems.Contains(availableCostItems[i])) continue;
            if(excludeItems.Contains(availableCostItems[i])) continue;
            if(!rarity.Contains(availableCostItems[i].itemRarity)) continue;
            if(!category.Contains(availableCostItems[i].itemCategory)) continue;
            acceptedCostItems.Add(availableCostItems[i]);
        }  

        return acceptedCostItems;
    }

    Item GetCostItem(List<Item> acceptedCostItems)
    {
        if(acceptedCostItems.Count <= 0) return null;
        int r = Random.Range(0, acceptedCostItems.Count);
        Item item = acceptedCostItems[r];
        acceptedCostItems.RemoveAt(r);
        return item;
    }

    Item GetTradeItem(ItemRarity rarity, ItemCategory[] category)
    {
        return GameManager.instance.itemLibrary.GetRandomItem(rarity, category);
    }

    #endregion

    #region  UI

    void SetupUi()
    {
        foreach (TizoTradeModule module in tizoTradeModules)
        {
            module.gameObject.SetActive(false);
        }

        if(trades.Count <= 0) return;

        int m = 0;
        for (int i = 0; i < trades.Count; i++)
        {
            if(m > tizoTradeModules.Length) break;
            if(trades[i] == null) continue;
            tizoTradeModules[m].SetTradeModule(trades[i], i, this);
            tizoTradeModules[m].gameObject.SetActive(true);
            m++;
        }
        m--;
        
        Navigation nav = tizoTradeModules[0].GetComponent<Selectable>().navigation;
        Navigation nav2 = tizoTradeModules[m].GetComponent<Selectable>().navigation;
        
        nav.selectOnUp = tizoTradeModules[m].GetComponent<Selectable>();
        nav2.selectOnDown = tizoTradeModules[0].GetComponent<Selectable>();

        tizoTradeModules[0].GetComponent<Selectable>().navigation = nav;
        tizoTradeModules[m].GetComponent<Selectable>().navigation = nav2;
    }

    void UpdateUi()
    {
        foreach (TizoInventoryItem tizoItem in inventoryItemImages) // Reset all items
        {
            tizoItem.gameObject.SetActive(false);
            tizoItem.amount = 0;
            tizoItem.amountText.text = "";
            tizoItem.amountText.gameObject.SetActive(false);
            tizoItem.itemName = "";
        }

        List<Item> inventoryItems = Player.instance.playerInventory.getItems();
        int inventoryItemPointer = 0;

        foreach (Item item in inventoryItems) //find item amounts, if new item populate
        {
            bool isNewItem = true;
            foreach(TizoInventoryItem tizoItem in inventoryItemImages)
            {
                if(tizoItem.itemName == item.name)
                {
                    isNewItem = false;
                    tizoItem.amount++;
                    break;
                }
            } 
            if(isNewItem)
            {
                inventoryItemImages[inventoryItemPointer].itemName = item.name;
                inventoryItemImages[inventoryItemPointer].image.sprite = item.sprite;
                inventoryItemImages[inventoryItemPointer].amount = 1;
                inventoryItemPointer++;
            }
        }

        foreach(TizoInventoryItem tizoItem in inventoryItemImages) //show items, and amount if 2 or more
        {
            
            if(tizoItem.amount > 1)
            {
                tizoItem.amountText.text = ""+tizoItem.amount;
                tizoItem.amountText.gameObject.SetActive(true);
                tizoItem.gameObject.SetActive(true);
            } else if (tizoItem.amount == 1)
            {
                tizoItem.gameObject.SetActive(true);
            } 
        }

        for (int i = 0; i < trades.Count && i < tizoTradeModules.Length; i++)
        {
            tizoTradeModules[i].UpdateUi();
        }
    }

    public void UpdateInfoCards(TizoTrade trade)
    {
        foreach (GameObject obj in infoCards)
        {
            obj.SetActive(false);
        }

        int c = 0;

        for (int i = 0; i < trade.getItems.Length && c < infoCards.Length; i++)
        {
            infoCards[c].GetComponentInChildren<Image>().sprite = trade.getItems[i].sprite;
            infoCards[c].GetComponentsInChildren<TMP_Text>()[0].text = trade.getItems[i].GetName();
            infoCards[c].GetComponentsInChildren<TMP_Text>()[1].text = trade.getItems[i].GetDesc();
            infoCards[c].GetComponentsInChildren<TMP_Text>()[0].color = Color.white;
            infoCards[c].SetActive(true);
            c++;
        }

        infoSeparator.transform.SetSiblingIndex(trade.getItems.Length+1);

        for (int i = 0; i < trade.costItems.Length && c < infoCards.Length; i++)
        {
            infoCards[c].GetComponentInChildren<Image>().sprite = trade.costItems[i].sprite;
            infoCards[c].GetComponentsInChildren<TMP_Text>()[0].text = trade.costItems[i].GetName();
            infoCards[c].GetComponentsInChildren<TMP_Text>()[1].text = trade.costItems[i].GetDesc();
            if (!Player.instance.playerInventory.HasItemWithName(trade.costItems[i].name))
            {
                infoCards[c].GetComponentsInChildren<TMP_Text>()[0].text += " " + missingString.GetLocalizedString();
                infoCards[c].GetComponentsInChildren<TMP_Text>()[0].color = Color.red;
            }
            else
            {
                infoCards[c].GetComponentsInChildren<TMP_Text>()[0].color = Color.white;
            }
            infoCards[c].SetActive(true);
            c++;
        }
    }

    #endregion

    public bool TryTrade(int tradeIndex)
    {
        if(tradeIndex > trades.Count || tradeIndex < 0)
        {
            Debug.LogError("Tizo trade index out of range");
            return false;
        }

        TizoTrade trade = trades[tradeIndex];
        for (int i = 0; i < trade.costItems.Length; i++)
        {
            if(!Player.instance.playerInventory.HasItemWithName(trade.costItems[i].name)) return false;
        }

        for (int i = 0; i < trade.costItems.Length; i++)
        {
            Player.instance.playerInventory.RemoveItemWithName(trade.costItems[i].name);
        }

        for (int i = 0; i < trade.getItems.Length; i++)
        {
            Player.instance.playerInventory.AddItem(trade.getItems[i]);
            //TODO we need to check so that we can actually add all the items (so that we don't get too many)
        }

        UpdateUi();

        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);

        TizoTradeModule nextTrade = null;

        for (int i = 1; i < tizoTradeModules.Length && i < trades.Count; i++)
        {   
            int indx = (tradeIndex+i)%((trades.Count < tizoTradeModules.Length)?trades.Count:tizoTradeModules.Length);
            TizoTradeModule module = tizoTradeModules[indx];
            if(module.hasBought) continue;
            nextTrade = module;
            break;
        }
        
        if(nextTrade) 
        {
            Navigation nav = tizoTradeModules[tradeIndex].GetComponent<Selectable>().navigation.selectOnUp.navigation;
            nav.selectOnDown = nextTrade.GetComponent<Selectable>();
            Navigation nav2 =  nextTrade.GetComponent<Selectable>().navigation;
            nav2.selectOnUp = tizoTradeModules[tradeIndex].GetComponent<Selectable>().navigation.selectOnUp;

            tizoTradeModules[tradeIndex].GetComponent<Selectable>().navigation.selectOnUp.navigation = nav;
            nextTrade.GetComponent<Selectable>().navigation = nav2;

            nextTrade.GetComponent<Selectable>().Select();
        }
        else ShopInteract(false);

        return true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus && mainComponent.activeSelf)
        {
            StartCoroutine(FocusGained());
        }
    }

    IEnumerator FocusGained()
    {
        yield return null;
        tizoTradeModules[0].GetComponent<Selectable>().Select();
    }
}

[System.Serializable]
class TizoTradeBlueprint
{
    public ItemRarity[] costRarity;
    public ItemCategory[] costCategory;
    public int costAmount;
    public ItemRarity[] getRarity;
    public ItemCategory[] getCategory;
    public int getAmount;
    public Item[] guaranteedItems;
}

public class TizoTrade
{
    public ItemRarity tradeRarity;
    public Item[] costItems;
    public Item[] getItems;

    public TizoTrade(ItemRarity rarity, Item[] costItems, Item[] getItems)
    {
        this.tradeRarity = rarity;
        this.costItems = costItems;
        this.getItems = getItems;
    }
}
