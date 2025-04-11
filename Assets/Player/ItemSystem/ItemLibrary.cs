using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UIElements;
public class ItemLibrary : MonoBehaviour{

    [SerializeField] SerializedDictionary<ItemRarity, SerializedDictionary<ItemRarity, float>> dropPointRarities;
    private ItemCollection[,] itemMatrix = new ItemCollection[0,0];
    private Dictionary<string, Item> specialItems = new Dictionary<string, Item>();

    void Awake()
    {
        PopulateMatrix();   
    }
    public void PopulateMatrix()
    {
        PopulateSpecialItems();

        if(itemMatrix.Length > 0) return; 
        itemMatrix = new ItemCollection[4,3];

        Debug.Log("--- Populating Item Matrix ------------------");
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Item[] items = Resources.LoadAll<Item>("Items/"+((ItemCategory)i).ToString()+"/"+((ItemRarity)j).ToString());
                Debug.Log("Items/"+((ItemCategory)i).ToString()+"/"+((ItemRarity)j).ToString() + " ---->  matrix[" + i + ":" + j + "], item count = " + items.Length);
                itemMatrix[i,j] = new ItemCollection(items);
            }
        }
        Debug.Log("--- Item Population Done ----------------");
    }

    void PopulateSpecialItems()
    {
        specialItems = new Dictionary<string, Item>();
        Item[] getSpecial = Resources.LoadAll<Item>("Items/_Special/");
        foreach (Item item in getSpecial)
        {
            specialItems.Add(item.name, item);
        }
    }

    public ItemCollection[] GetCategory(ItemCategory category)
    {
        ItemCollection[] ret = new ItemCollection[3];
        for (int i = 0; i < 3; i++)
        {
            ret[i] = itemMatrix[(int)category, i];
        }

        return ret;
    }

    public ItemCollection[] GetRarity (ItemRarity rarity)
    {
        ItemCollection[] ret = new ItemCollection[3];
        for (int i = 0; i < 4; i++)
        {
            ret[i] = itemMatrix[i, (int)rarity];
        }

        return ret;
    }

    public ItemCollection GetCollection(ItemCategory category, ItemRarity rarity)
    {
        if(itemMatrix.Length <= 0) PopulateMatrix(); 
        return itemMatrix[(int)category, (int)rarity];
    }

    public ItemCollection[,] GetSubMatrix(ItemCategory[] includeCategory, ItemRarity[] includeRarity)
    {
        ItemCollection[,] ret = new ItemCollection[includeCategory.Length,includeRarity.Length];

        for (int i = 0; i < includeCategory.Length; i++)
        {
            for (int j = 0; j < includeRarity.Length; j++)
            {
                ret[i,j] = itemMatrix[(int)includeCategory[i],(int)includeCategory[j]];
            }
        }

        return ret;
    }
    
    public Item GetItem(ItemCategory itemCategory, ItemRarity itemRarity, string itemName)
    {
        if(itemCategory == ItemCategory.Special)
        {
            return specialItems[itemName];
        }

        return GetCollection(itemCategory, itemRarity).itemDictionary[itemName];
    }

    /// <summary>
    /// Get a random item. The item will come from one of the included categories and the rarity will be weighted by the drop point rarity;
    /// </summary>
    /// <param name="droppointRarity"></param>
    /// <param name="includeCategories"></param>
    /// <returns></returns>
    public Item GetRandomItem(ItemRarity dropPointRarity, ItemCategory[] includeCategories)
    {
        if(includeCategories.Length == 0)
        {
            Debug.LogWarning("Included item categories is empty");
            return null;
        }

        //Decide weight depending on droppioint rarity
        float[] weight = new float[3];
        for (int i = 0; i < 3; i++)
        {
            weight[i] = dropPointRarities[dropPointRarity][(ItemRarity)i];
        }

        //Pick a random category depending on which are available
        int categoryIndex = Random.Range(0, includeCategories.Length);
        ItemCollection[] category = GetCategory(includeCategories[categoryIndex]);

        //Pick a random rarity determined by the weight
        float randomRarity = Random.Range(1f, 100f)/100f;
        int picker = -1;
        for (int i = 0; i < 3; i++)
        {
            if(randomRarity <= weight[i])
            {
                picker = i;
                break;
            }

            randomRarity -= weight[i];
        }

        //If the random rarity could not be chosen something has gone incredibly wrong.
        if(picker == -1)
        {
            Debug.LogError("Critical error in selecting random item. Picker out if range");
            return null;
        }

        //Pick a random item from the category and rarity
        List<Item> items = new List<Item>(category[picker].items.Length);
        items.AddRange(category[picker].items);

        if(items.Count == 0)
        {
            items = new List<Item>(category[(int)dropPointRarity].items.Length);
            Debug.LogWarning("Item collection " + ((ItemCategory)categoryIndex).ToString()+"/"+((ItemRarity)picker).ToString() + " is empty!, Taking from default");
            items.AddRange(category[(int)dropPointRarity].items);

            if(items.Count == 0) 
            {
                Debug.LogError("Item collection " + ((ItemCategory)categoryIndex).ToString() + "/" + dropPointRarity.ToString() + " is empty even though it is default!");
                items = new List<Item>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < itemMatrix[i,(int)dropPointRarity].items.Length; j++)
                    {
                        items.Add(itemMatrix[i,(int)dropPointRarity].items[j]);;
                    }
                }
            }
        }

        Item ret = null;
        while(!ret)
        {
            if(items.Count == 0)
            {
                Debug.LogWarning("All items in collection " + ((ItemCategory)categoryIndex).ToString()+"/"+((ItemRarity)picker).ToString() + "  are unspawnable");
                return null;
            }

            int itemIndex = Random.Range(0, items.Count);
            if(!items[itemIndex].CanBeSpawned())
            {
                items.RemoveAt(itemIndex);
                continue;
            } else
            {
                ret = items[itemIndex];
                break;
            }
        }

        return ret;
    }
}

public class ItemCollection
{
    public ItemCollection(Item[] items)
    {
        this.items = items;

        itemDictionary = new Dictionary<string, Item>();
        foreach (Item item in items)
        {
            itemDictionary.Add(item.name, item);
        }
    }
    public Item[] items;
    public Dictionary<string, Item> itemDictionary;
}

/// <summary>
/// What category the item is in
/// </summary>
public enum ItemCategory
{
    ColorBoost,
    Damage,
    Survival,
    Utility,
    Special
}

/// <summary>
/// How rare the item is
/// </summary>
public enum ItemRarity
{
    Common,
    Rare,
    Mythic
}
