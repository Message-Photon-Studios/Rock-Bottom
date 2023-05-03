using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinController : MonoBehaviour
{
    private ItemInventory inventory;

    [SerializeField] TMP_Text coins;
   
   private void OnEnable() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        inventory.onCoinsChanged += UpdateCoins;
        UpdateCoins();
   }

    private void UpdateCoins() {
        coins.text =  "" + inventory.GetCoins();
    }

    private void UpdateCoins(int change) {
        UpdateCoins();
    }

   private void OnDisable() {
    
   }
}
