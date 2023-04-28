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
    }
}

#endif