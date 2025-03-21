using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DeathShrine : InteractionObject
{
    [SerializeField] int healthCost = 0;
    [SerializeField] ItemPickup itemPickup;
    [SerializeField] GameObject canvasObject;


    bool bought = false;


    protected override void PlayerClose(bool isClose)
    {
        if(bought) return;

        canvasObject.SetActive(isClose);
    }

    protected override void PlayerInteract()
    {
        if(bought) return;

        Player.instance.playerStats.RemoveMaxHealth(healthCost);
        itemPickup.gameObject.SetActive(true);
        canvasObject.SetActive(false);

        bought = true;
    }
}
