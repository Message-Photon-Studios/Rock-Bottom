using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.Localization;

public class HealingShrine : InteractionObject
{
    [SerializeField] int heal;
    [SerializeField] int baseCost;
    [SerializeField] int increaseCost;
    [SerializeField] PickUpCanvasController pickUpController;
    [SerializeField] LocalizedString healingShrineName;
    [SerializeField] List<LocalizedString> phrases;
    [SerializeField] LocalizedString healAmoutText;
    Animator animator;



    Action<InputAction.CallbackContext> buy;

    private ItemInventory inventory;
    private PlayerStats player;
    private int count = 0;

    protected override void Start()
    {
        base.Start();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
    }

    public int CalculatePrice()
    {
        return Mathf.RoundToInt((baseCost + increaseCost * count) * ItemSpellManager.instance.stageCostMultiplier * GameManager.instance.rerunNum);
    }

    protected override void PlayerClose(bool isClose)
    {
        if(isClose) pickUpController.SetHealthShrine(this);
        else pickUpController.CloseMenu();
    }

    protected override void PlayerInteract()
    {
        if (animator.GetBool("heal")) return;
        if(Player.instance.playerStats.GetHealth() >= Player.instance.playerStats.GetMaxHealth()) return;
        if(inventory.PayCost(CalculatePrice()))
        {
            player.HealPlayer(heal);
            AchievementsManager.instance?.onHealingShrineUsed?.Invoke();
            count++;
            pickUpController.UpdateHealthShrine(this);
            animator.SetBool("heal", true);
        }
    }

    protected void HealDone()
    {
        animator.SetBool("heal", false);
    }

    public string GetName()
    {
        return healingShrineName.GetLocalizedString();
    }

    public string GetDescription()
    {
        string ret = "";
        if (count >= phrases.Count) ret = phrases[phrases.Count-1].GetLocalizedString() + "\n" + healAmoutText.GetLocalizedString();
        else ret = phrases[count].GetLocalizedString() + "\n" + healAmoutText.GetLocalizedString();
        return ret;
    }
}
