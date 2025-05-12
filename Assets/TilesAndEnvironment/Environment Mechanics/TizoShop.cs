using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TizoShop : MonoBehaviour
{   
    [Header("Ui")]
    [SerializeField] GameObject canvas;
    [SerializeField] TizoTradeModule[] tizoTradeModules;
    [SerializeField] EventSystem eventSystem;

    [Header("Shop Settings")]
    [SerializeField] int commonTradeAmount = 5;
    [SerializeField] int rareTradeAmount = 4;
    [SerializeField] int mysticTradeAmount = 1;

    [Header("Trade Blueprints")]
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
            Player.instance.playerMovement.movementRoot.SetTotalRoot("tizoShop", true);
            canvas.SetActive(true);
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            tizoTradeModules[0].GetComponent<Selectable>().Select();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else if (shopOpen && !openShop)
        {
            shopOpen = false;
            Player.instance.playerMovement.movementRoot.SetTotalRoot("tizoShop", false);
            canvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Start()
    {
        shopOpen = false;
        canvas.SetActive(false);
        trades = new List<TizoTrade>();

        SetAvailableCostItems();
        SetTrades();
        SetupUi();
    }

    void SetAvailableCostItems()
    {
        availableCostItems = new List<Item>();
        availableCostItems.AddRange(Player.instance.playerInventory.getItems());

        //TODO add so that the trades can be from items found in level as well
    }

    void SetTrades()
    {
        for (int i = 0; i < commonTradeAmount; i++)
        {
            trades.Add(CreateTrade(commonTradeBlueprints[Random.Range(0, commonTradeBlueprints.Length)], ItemRarity.Common));
        }

        for (int i = 0; i < rareTradeAmount; i++)
        {
            trades.Add(CreateTrade(rareTradeBlueprints[Random.Range(0, rareTradeBlueprints.Length)], ItemRarity.Rare));
        }

        for (int i = 0; i < mysticTradeAmount; i++)
        {
            trades.Add(CreateTrade(mysticTradeBlueprints[Random.Range(0, mysticTradeBlueprints.Length)], ItemRarity.Mythic));
        }
    }

    TizoTrade CreateTrade(TizoTradeBlueprint blueprint, ItemRarity rarity)
    {
        List<Item> acceptedCostItems = GetAcceptedCostItems(blueprint.costRarity.ToList(), blueprint.costCategory.ToList());

        Item[] costItems = new Item[blueprint.costAmount];
        for (int i = 0; i < blueprint.costAmount; i++)
        {
            costItems[i] = GetCostItem(acceptedCostItems);
            if(costItems[i] == null) return null; 
        }

        Item[] getItems = new Item[blueprint.getAmount];
        for (int i = 0; i < blueprint.getAmount; i++)
        {
            getItems[i] = GetTradeItem(blueprint.getRarity[Random.Range(0, blueprint.getRarity.Length)], blueprint.getCategory);
        }

        return new TizoTrade(rarity, costItems, getItems);
    }

    List<Item> GetAcceptedCostItems(List<ItemRarity> rarity, List<ItemCategory>category)
    {
        List<Item> acceptedCostItems = new List<Item>();
        for (int i = 0; i < availableCostItems.Count; i++)
        {
            if(acceptedCostItems.Contains(availableCostItems[i])) continue;
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

        int m = 0;
        for (int i = 0; i < trades.Count; i++)
        {
            if(m > tizoTradeModules.Length) break;
            if(trades[i] == null) continue;
            tizoTradeModules[m].SetTradeModule(trades[i], i, this);
            tizoTradeModules[m].gameObject.SetActive(true);
            m++;
        }
    }

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
            if(!Player.instance.playerInventory.HasItemWithName(trade.costItems[i].GetName())) return false;
        }

        for (int i = 0; i < trade.costItems.Length; i++)
        {
            Player.instance.playerInventory.RemoveItemWithName(trade.costItems[i].GetName());
        }

        for (int i = 0; i < trade.getItems.Length; i++)
        {
            Player.instance.playerInventory.AddItem(trade.getItems[i]);
            //TODO we need to check so that we can actually add all the items (so that we don't get too many)
        }

        return true;
    }



    #endregion
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
