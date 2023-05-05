using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsController : MonoBehaviour
{
    PlayerStats playerStats;
    ItemInventory inventory;
    [SerializeField] TMP_Text maxHealth;
    [SerializeField] TMP_Text coins;

   ColorInventory colorInventory;
   [SerializeField] GameColor[] colors;
   [SerializeField] TMP_Text[] colorNumbers;

   private void OnEnable() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        colorInventory = colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        UpdateStats();
        UpdateColorNumbers();
   }

   private void OnDisable() {
    
   }

    private void UpdateStats(){
        maxHealth.text = playerStats.GetMaxHealth() + "hp";
        coins.text = "" + inventory.GetCoins();
    }

    private void UpdateColorNumbers() {
        for(int i = 0; i < 7; i++) {
            float number = 100*(1 + colorInventory.GetColorBuff(colors[i]));
            colorNumbers[i].text = number + "%";
        }
    }
}
