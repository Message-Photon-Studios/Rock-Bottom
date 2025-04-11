using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DeathShrine : MonoBehaviour
{
    [SerializeField] int healthCost = 0;
    [SerializeField] InputActionReference buyAction;
    [SerializeField] ItemPickup itemPickup;
    [SerializeField] GameObject canvasObject;
    Action<InputAction.CallbackContext> buy;

    bool buyable = false;

    bool bought = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        buy = (InputAction.CallbackContext ctx) => {Buy(); };
        buyAction.action.performed += buy;
    }

    private void OnDisable()
    {
        buyAction.action.performed -= buy;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !bought)
        {
            buyable = true;
            canvasObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !bought)
        {
            buyable = false;
            canvasObject.SetActive(false);
        }
    }

    void Buy()
    {
        if(!buyable || bought) return;

        PlayerLevelMananger.instance.playerStats.RemoveMaxHealth(healthCost);
        itemPickup.gameObject.SetActive(true);
        canvasObject.SetActive(false);

        bought = true;
    }
}
