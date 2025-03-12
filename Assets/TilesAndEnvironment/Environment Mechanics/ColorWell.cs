using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ColorWell : MonoBehaviour
{
    public GameColor color;
    [SerializeField] int colorAmount;
    private bool playerClose = false;
    public bool wellUsed {get; private set; } = false; 

    void OnDisable()
    {
        playerClose = false;
    }

    public int GetColorAmount()
    {
        if(color != null && !wellUsed) return colorAmount;
        else return 0;
    }

    public void UseWell()
    {
        wellUsed = true;
        playerClose = false;
    }
    
    #region Check playerClose
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = true;
            PlayerLevelMananger.instance.playerCombatSystem.EnableAbsorbColor(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = false;
            PlayerLevelMananger.instance.playerCombatSystem.DisableAbsorbColor();
        }
    }
    #endregion
}
