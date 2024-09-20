using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// This is the base class for an item
/// </summary>
[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    [TextArea(5,20)] public string description;
    public int itemCost;
    [SerializeReference] public List<ItemEffect> effects = new List<ItemEffect>();

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
        return ret;
    }
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

        if(GUILayout.Button("Add enemy color don't mix"))
        {
            item.effects.Add(new EnemyDontMixItem());
        }

        if(GUILayout.Button("Add brush damage"))
        {
            item.effects.Add(new BrushDamageItem());
        }

        if(GUILayout.Button("Add color armour"))
        {
            item.effects.Add(new ColorArmour());
        }
    }
}

#endif