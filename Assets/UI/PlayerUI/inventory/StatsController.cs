using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsController : MonoBehaviour
{
    //Players stats.
    PlayerStats playerStats;

    //Players inventory.
    ItemInventory inventory;

    //Text field for players maxHealth.
    [SerializeField] TMP_Text maxHealth;

    //Text field for players coins.
    [SerializeField] TMP_Text coins;

    //Players color inventory.
    ColorInventory colorInventory;

    //All the colors in the game.
    [SerializeField] GameColor[] colors;

    //All the text fields for the power of the colors.
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

    /// <summary>
    /// Updates stats in the stat portion.
    /// </summary>
    private void UpdateStats(){
        maxHealth.text = playerStats.GetMaxHealth() + "hp";
        coins.text = "" + inventory.GetCoins();
    }

    /// <summary>
    /// Updates color power% in the Pigment Power section.
    /// </summary>
    private void UpdateColorNumbers() {
        for(int i = 0; i < 7; i++) {
            float number = 100*(1 + colorInventory.GetColorBuff(colors[i]));
            colorNumbers[i].text = number + "%";
        }
    }
}
