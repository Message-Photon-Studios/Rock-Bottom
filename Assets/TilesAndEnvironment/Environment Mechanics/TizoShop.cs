using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TizoShop : MonoBehaviour
{   
    [Header("Ui")]
    [SerializeField] GameObject canvas;
    [SerializeField] TizoTradeModule[] tizoTradeModules;
    [SerializeField] Image[] inventoryItemImages;
    [SerializeField] GameObject[] infoCards;

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
            UpdateUi();
            canvas.SetActive(true);
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            tizoTradeModules[0].GetComponent<Selectable>().Select();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            UpdateInfoCards(trades[0]);
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

    void UpdateUi()
    {
        foreach (Image itemImage in inventoryItemImages)
        {
            itemImage.gameObject.SetActive(false);
        }

        List<Item> inventoryItems = Player.instance.playerInventory.getItems();

        for (int i = 0; i < inventoryItems.Count && i < inventoryItemImages.Length; i++)
        {
            inventoryItemImages[i].sprite = inventoryItems[i].sprite;
            inventoryItemImages[i].gameObject.SetActive(true);
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
            infoCards[c].SetActive(true);
            c++;
        }

        for (int i = 0; i < trade.costItems.Length && c < infoCards.Length; i++)
        {
            infoCards[c].GetComponentInChildren<Image>().sprite = trade.costItems[i].sprite;
            infoCards[c].GetComponentsInChildren<TMP_Text>()[0].text = trade.costItems[i].GetName();
            infoCards[c].GetComponentsInChildren<TMP_Text>()[1].text = trade.costItems[i].GetDesc();
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

        return true;
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
