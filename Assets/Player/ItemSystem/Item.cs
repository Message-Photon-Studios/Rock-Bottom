using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using System;
using UnityEngine.Localization;
using Unity.VisualScripting;
using UnityEditor.Localization.Editor;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

/// <summary>
/// This is the base class for an item
/// </summary>
[CreateAssetMenu(menuName = "Item/Item"), Serializable]
public class Item : ScriptableObject
{
    public Sprite sprite;
    [SerializeField] public LocalizedString itemName;
    [SerializeField] public LocalizedString description;
    [SerializeField] public LocalizedString longDescription;
    [SerializeField] public LocalizedString loreDescription;
    public int itemCost;
    [SerializeField] private IntVariable[] totbuffVariables;
    [SerializeField] private IntVariable[] buffVariables;
    [SerializeReference] public List<ItemEffect> effects = new List<ItemEffect>(); 
    [SerializeField] public ItemCategory itemCategory;
    [SerializeField] public ItemRarity itemRarity;
    [SerializeField] public float maxSpawn;

    public void EnableItem() 
    {
        foreach (ItemEffect item in effects)
        {
            item.ActivateEffect();
        }
    }

    public void DisableItem()
    {
        foreach(ItemEffect item in effects)
        {
            item.DisableEffect();
        }
    }

    /// <summary>
    /// Returns true if the item can be spawned.
    /// </summary>
    /// <returns></returns> <summary>
    public bool CanBeSpawned()
    {
        bool ret = true;
        foreach (ItemEffect item in effects)
        {
            ret &= item.CanBeSpawned();
        }
        if (maxSpawn != -1 && ret)
        {
            int count = 0;
            foreach (Item item in Player.instance.playerInventory.getItems())
            {
                if (this.name.Equals(item.name)) count++;
                
            }

            count += ItemSpellManager.instance.GetItemsSpawned(this);
            if (count >= maxSpawn) ret = false;
        }
        return ret;
    }

    /// <summary>
    /// Gives name of item depending on which language is selected.
    /// </summary>
    /// <returns>Localized Name.</returns>
    public string GetName() {
        return itemName.GetLocalizedString();
    }

    /// <summary>
    /// Gives description of item depending on which language is selected.
    /// </summary>
    /// <returns>Localized description.</returns>
    public string GetDesc() {
        for (int i = 0; i < totbuffVariables.Length; i++)
        {
            Variable<string> setter = new Variable<string>();
            description.Add("totbuff_"+i, totbuffVariables[i]);
        }

        for (int i = 0; i < buffVariables.Length; i++)
        {
            description.Add("buff_"+i, buffVariables[i]);
        }
        return description.GetLocalizedString();
    }

    public string GetLongDesc()
    {        for (int i = 0; i < totbuffVariables.Length; i++)
        {
            Variable<string> setter = new Variable<string>();
            longDescription.Add("totbuff_"+i, totbuffVariables[i]);
        }

        for (int i = 0; i < buffVariables.Length; i++)
        {
            longDescription.Add("buff_"+i, buffVariables[i]);
        }
        if(longDescription.IsEmpty) return null;
        return longDescription.GetLocalizedString();
    }

    public string GetLoreDesc()
    {
        if(loreDescription.IsEmpty) return null;
        return loreDescription.GetLocalizedString();
    }

    #if UNITY_EDITOR

    //Ensures that the item category and item rarity variables are always set correctly, dependent on what folder the item is in.
    private void OnValidate() 
    {
        if(itemCategory == ItemCategory._Special) return;
        string currentPath = AssetDatabase.GetAssetPath(this);
        string[] splitString = currentPath.Split('/');
        
        System.Object temp; 
        Enum.TryParse(typeof(ItemCategory), splitString[3], out temp);
        itemCategory = (ItemCategory)temp;

        System.Object temp2; 
        Enum.TryParse(typeof(ItemRarity), splitString[4], out temp2);
        itemRarity = (ItemRarity)temp2;
    }
    #endif

}

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
public class ItemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Item item = (Item)target;
        if(GUILayout.Button("Add color buff"))
        {
            item.effects.Add(new ColorBuffItem());
        }

        if(GUILayout.Button("Add health"))
        {
            item.effects.Add(new ChangeHealthIItem());
        }

        
        if(GUILayout.Button("Add spell slot"))
        {
            item.effects.Add(new AddSpellSlot());
        }

        if(GUILayout.Button("Add coin boost"))
        {
            item.effects.Add(new CoinBoostItem());
        }

        if (GUILayout.Button("Add block damage"))
        {
            item.effects.Add(new BlockDamageItem());
        }

        if (GUILayout.Button("Add block drain color"))
        {
            item.effects.Add(new BlockUseColor());
        }

        if(GUILayout.Button("Add Color Nearby"))
        {
            item.effects.Add(new ColorNearbyUncolored());
        }

        if(GUILayout.Button("Add Rainbow Maxed buff"))
        {
            item.effects.Add(new RainbowMaxedBuff());
        }

        if(GUILayout.Button("Add brush damage"))
        {
            item.effects.Add(new BrushDamageItem());
        }

        if(GUILayout.Button("Add color armour"))
        {
            item.effects.Add(new ColorArmour());
        }

        if(GUILayout.Button("Add cascading damage"))
        {
            item.effects.Add(new CascadingDamage());
        }

        if (GUILayout.Button("Add Cool Down Multiplier"))
        {
            item.effects.Add(new ChangeCDMultiplier());
        }

        if (GUILayout.Button("Add Color Balance Setter"))
        {
            item.effects.Add(new BalanceAllColors());
        }
        if (GUILayout.Button("Add Random Buff Setter"))
        {
            item.effects.Add(new RandomBuff());
        }
        if (GUILayout.Button("Add On Custom Effect"))
        {
            item.effects.Add(new CustomItemEffect());
        }
        if (GUILayout.Button("Add Color Inventory Bools Manager"))
        {
            item.effects.Add(new ColorInventoryBools());
        }
        if (GUILayout.Button("Add Compliment Damage Bonus"))
        {
            item.effects.Add(new ComplimentaryDamage());
        }
        if (GUILayout.Button("Add Player Stats Bools Manager"))
        {
            item.effects.Add(new PlayerStatsBools());
        }
        if (GUILayout.Button("Add Change Stored Spell Bonus"))
        {
            item.effects.Add(new ChangeStoredSpellBonus());
        }
        if (GUILayout.Button("Add Color Max Bonus"))
        {
            item.effects.Add(new ColorMaxBonus());
        }
        if (GUILayout.Button("Add Color Max Damage Bonus"))
        {
            item.effects.Add(new ColorMaxDamageBonus());
        }
        if (GUILayout.Button("Add Adaptive Color Armour"))
        {
            item.effects.Add(new AdaptiveColorArmour());
        }
        if (GUILayout.Button("Add Invincibility Bonus"))
        {
            item.effects.Add(new AddInvincibilityBonus());
        }
        if (GUILayout.Button("Add Spell Bonus Damage"))
        {
            item.effects.Add(new AddSpellBonusDamage());
        }
        if (GUILayout.Button("Add Change THP"))
        {
            item.effects.Add(new ChangeTemporaryHealth());
        }
        if (GUILayout.Button("Add Concentraded Color Buff"))
        {
            item.effects.Add(new ConcentradedColorBuff());
        }
        if (GUILayout.Button("Add Emergency Brush Buff"))
        {
            item.effects.Add(new EmergencyBrushBuff());
        }
        if (GUILayout.Button("Add D6 Damage Buff"))
        {
            item.effects.Add(new D6DamageBuff());
        }
    }
}

#endif