using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ColorWell : MonoBehaviour
{
    [SerializeField] int colorAmount;
    private bool playerClose = false;
    private bool wellUsed = false; 

    void OnDisable()
    {
        playerClose = false;
    }
    
    #region Check playerClose
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = false;
        }
    }
    #endregion
}
