using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class WillowCrate : InteractionObject
{
    [Header("Crate Variables")]
    public SpawnPointChance spawnChance = SpawnPointChance.LowChance;
    [SerializeField] ItemPickup itemPickup;
    [SerializeField] ItemPickup keyPickup;
    [SerializeField] float keyChance;
    [SerializeField] float cost;

    [Header("LocalizedStrings")]
    [SerializeField] LocalizedString willowCrateName;
    [SerializeField] LocalizedString willowCrateDesc;

    [Header("UI")]
    [SerializeField] PickUpCanvasController pickUpCanvas;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject mapIcon;

    int actualCost = 0;
    bool unlocked = false;

    protected override void Start()
    {
        base.Start();
        itemPickup.gameObject.SetActive(false);
        keyPickup.gameObject.SetActive(false);
        actualCost = (int)(cost * ItemSpellManager.instance.stageCostMultiplier * GameManager.instance.rerunNum);
        mapIcon.SetActive(true);
        unlocked = false;
    }

    protected override void PlayerInteract()
    {
        if(unlocked) return;
        if(!Player.instance.playerInventory.PayCost(actualCost)) return;

        float r = UnityEngine.Random.Range(0, 100f)/100f;
        if(r <= keyChance)
        {
            keyPickup.gameObject.SetActive(true);
        } else
        {
            itemPickup.gameObject.SetActive(true);
        }

        sprite.SetActive(false);
        pickUpCanvas.gameObject.SetActive(false);
        mapIcon.SetActive(false);
        unlocked = true;
    }

    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);
        if(unlocked) return;
        if(isClose)
        {
            pickUpCanvas.SetCrate(willowCrateName.GetLocalizedString(), willowCrateDesc.GetLocalizedString(), actualCost);
        } else
        {
            pickUpCanvas.CloseMenu();
        }
    }
}
