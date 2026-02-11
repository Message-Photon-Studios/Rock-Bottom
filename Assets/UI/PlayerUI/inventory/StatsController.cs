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
    [SerializeField] TMP_Text petrifiedPigment;

    //Players color inventory.
    ColorInventory colorInventory;

    //All the colors in the game.
    [SerializeField] GameColor[] colors;

    //All the text fields for the power of the colors.
    [SerializeField] TMP_Text[] colorNumbers;

    // All the text fields for the color defense numbers.
    [SerializeField] TMP_Text[] colorDefenceNumbers;

   private void OnEnable() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        colorInventory = colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        UpdateStats();
        UpdateColorNumbers();
        UpdateColorDefenceNumbers();
   }

   private void OnDisable() {
    
   }

    /// <summary>
    /// Updates stats in the stat portion.
    /// </summary>
    private void UpdateStats(){
        maxHealth.text = playerStats.GetMaxHealth() + "hp";
        coins.text = "" + inventory.GetCoins();
        //petrifiedPigment.text = "" + 
    }

    /// <summary>
    /// Updates color power% in the Pigment Power section.
    /// </summary>
    private void UpdateColorNumbers() {
        for(int i = 0; i < 7; i++) {
            float number = 100*colorInventory.GetColorBuff(colors[i]);
            number = Mathf.Round(number);
            if (number < 100)
            {
                colorNumbers[i].color = new Color(1,0.5f,0.5f,1);
            } else if (number > 100)
            {
                colorNumbers[i].color = new Color(0.5f, 1, 0.5f, 1);
            } else
            {
                colorNumbers[i].color = Color.white;
            }
            colorNumbers[i].text = (int) number + "%";
        }
    }

    /// <summary>
    /// Updates color defence % in the pigment section.
    /// </summary>
    private void UpdateColorDefenceNumbers()
    {
        for(int i = 0; i < 7; i++) {
            float number = 100*playerStats.GetColorArmour(colors[i]);
            number = Mathf.Round(number);
            colorDefenceNumbers[i].color = Color.white;
            colorDefenceNumbers[i].text = (int) number + "%";
        }
    }
}
