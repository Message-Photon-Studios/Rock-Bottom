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
    }
}

#endif