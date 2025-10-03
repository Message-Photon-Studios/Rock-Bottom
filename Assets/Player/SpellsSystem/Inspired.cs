using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class Inspired : InteractionObject
{
    [SerializeField] GameObject spellToEnable;
    [SerializeField] ColorSpell unlockSpell;
    [SerializeField] public int petrifiedPigmentCost;
    [SerializeField] Sprite inspiredSprite;
    [SerializeField] LocalizedString inspireText;

    [SerializeField] LocalizedString unlockString;
    [SerializeField] LocalizedString costString;

    [SerializeField] UIMenu ui;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text headerText;
    [SerializeField] Animator animator;

    private bool triggered;

    private UIController UI;

    protected override void Start() {
        base.Start();

        if(spellToEnable) spellToEnable.SetActive(false);
        if(GameManager.instance.IsSpellSpawnable(unlockSpell))
        {
            if(animator) animator.SetBool("inspired", true);
            if(spellToEnable)
            {
                spellToEnable.SetActive(true);
            }

            triggered = true;
            GetComponent<Collider2D>().enabled = false;
        } else 
        {
            triggered = false;
        }
        UI = Player.instance.playerUi;

        costText.text = costString.GetLocalizedString() + petrifiedPigmentCost;
        descriptionText.text = unlockSpell.description.GetLocalizedString();
        headerText.text = unlockString.GetLocalizedString() + unlockSpell.GetName();
        if (ui)
        {
            ui.CloseMenu();
        }
    }

    void OnEnable()
    {
        if(triggered)
        {
            if(animator) animator.SetBool("inspired", true);
        }
    }
    
    protected override void PlayerClose(bool isClose) {
        if (isClose)
        {
            if (!triggered && !GameManager.instance.IsSpellSpawnable(unlockSpell))
            {
                costText.text = costString.GetLocalizedString() + petrifiedPigmentCost;
                descriptionText.text = unlockSpell.description.GetLocalizedString();
                headerText.text = unlockString.GetLocalizedString() + unlockSpell.GetName();
                if (GameManager.instance.GetPetrifiedPigmentAmount() < petrifiedPigmentCost) costText.color = Color.red;
                else costText.color = Color.white;
            }
            ui.OpenMenu();
        }
        else
        {
            ui.CloseMenu();
        }
    }

    protected override void PlayerInteract()
    {
        if(GameManager.instance.TryRemovePetrifiedPigment(petrifiedPigmentCost))
        {
            TriggerUnlock();
        }
    }

    private void TriggerUnlock()
    {
        triggered = true;
        UI.inspired?.Invoke(inspiredSprite, inspireText.GetLocalizedString());
        GameManager.instance.UnlockedSpell(unlockSpell);
        if(spellToEnable) 
        {
            spellToEnable.SetActive(true);
            spellToEnable.GetComponent<SpellPickup>().SetSpell(unlockSpell);
        }
        GameManager.instance.AddInspiration(1);
        ui.CloseMenu();

        if(animator) animator.SetBool("inspired", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
