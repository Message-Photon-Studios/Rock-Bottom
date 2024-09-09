using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Inspired : MonoBehaviour
{
    [SerializeField] GameObject spellToEnable;
    [SerializeField] ColorSpell unlockSpell;
    [SerializeField] public int petrifiedPigmentCost;
    [SerializeField] Sprite spell;
    [SerializeField] String text;

    [SerializeField] GameObject ui;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text headerText;

    private ItemInventory inventory;
    private bool triggered;

    private UIController UI;

    public void OnEnable() {
        if(GameManager.instance.IsSpellSpawnable(unlockSpell))
        {
            if(spellToEnable)
            {
                spellToEnable.SetActive(true);
                spellToEnable.GetComponent<SpellPickup>().SetSpell(unlockSpell);
            }
            gameObject.SetActive(false);
        }

        if(spellToEnable) spellToEnable.SetActive(false);
        triggered = false;
        UI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();

        costText.text = "Cost: " + petrifiedPigmentCost;
        descriptionText.text = unlockSpell.description;
        headerText.text = "Unlock " + unlockSpell.name;
        ui.SetActive(false);
    }
    
    public void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !triggered && !GameManager.instance.IsSpellSpawnable(unlockSpell)) {
            ui.SetActive(true);
            inventory.EnableInspired(this);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inventory.DisableInspired();
            ui.SetActive(false);
        }
    }

    public void TriggerUnlock()
    {
        triggered = true;
        UI.inspired(spell, text);
        GameManager.instance.UnlockedSpell(unlockSpell);
        if(spellToEnable) 
        {
            spellToEnable.SetActive(true);
            spellToEnable.GetComponent<SpellPickup>().SetSpell(unlockSpell);
        }
        gameObject.SetActive(false);
        
    }
}
