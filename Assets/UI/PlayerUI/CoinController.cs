using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CoinController : MonoBehaviour
{
    //Reference to players itemInventory
    private ItemInventory inventory;

    //Reference to text in UI
    [SerializeField] TMP_Text coins;
   
   private void OnEnable() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInventory>();
        inventory.onCoinsChanged += UpdateCoins;
        UpdateCoins();
   }

      private void OnDisable() {
    
   }

    /// <summary>
    /// If player coins is updated, fetch new number.
    /// </summary>
    private void UpdateCoins() {
        coins.text =  "" + inventory.GetCoins();
    }

    /// <summary>
    /// If player coins was changed, fetch new number and display change.
    /// </summary>
    /// <param name="change">amount changed, positive or negative.</param>
    private void UpdateCoins(int change) {
        UpdateCoins();
        StartCoroutine(CoinChange(change));
    }

    /// <summary>
    /// Given a number, creates a text corresponding to number 100 pixels below coins.
    /// Also moves the number towards coins position in Y axis in a deaccelerating maner at 
    /// a rate of 0.05*current position in Y. Then destroys the text.
    /// Also sets color to green if change is positive and red if negative.
    /// </summary>
    /// <param name="change">amound changed.</param>
    /// <returns></returns>
    IEnumerator CoinChange(int change) {
        int start = -100;
        int xPos = -40;
        TMP_Text coinChange = GameObject.Instantiate(coins) as TMP_Text;
        coinChange.GetComponent<RectTransform>().SetParent(coins.GetComponent<RectTransform>().parent); 

        coinChange.transform.localScale = coins.transform.localScale;
        coinChange.transform.localPosition = new Vector3(xPos, start);
        coinChange.text = "" + change;

        Color32 active;
        if(change < 0) {
            active = new Color32(220,0,0, 255);
        } else {
            active = new Color32(75,220,0, 255);
        }
        coinChange.color = active;

        int current = start;
        while(current < 0) {
            current = current - Mathf.Min(Mathf.CeilToInt(current*0.05f), -1);
            coinChange.transform.localPosition = new Vector3(xPos, current);
            active.a = (byte)(255*((float)current/start));
            coinChange.color = active;
            yield return new WaitForFixedUpdate();
        }

        Destroy(coinChange);
    }

}
