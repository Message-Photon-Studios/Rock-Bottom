using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PermanentItemShop : NpcUpgradeShop
{
    [SerializeField] private Item item;
    [SerializeField] SpriteRenderer displayImage;
    [SerializeField] TMP_Text itemDescription;
    [SerializeField] TMP_Text itemName;

    void Start()
    {
        displayImage.sprite = item.sprite;
        itemDescription.text = item.GetDesc();
        itemName.text = item.GetName();
    }

    protected override void Shop()
    {
        FindObjectOfType<ItemInventory>().AddItem(item);
        FindObjectOfType<ItemInventory>().SetPermanentItems();
        DataPersistenceManager.instance.SaveGame();
    }
}
