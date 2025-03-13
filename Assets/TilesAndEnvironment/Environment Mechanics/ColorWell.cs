using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorWell : MonoBehaviour
{
    public GameColor color;
    [SerializeField] int colorAmount;
    
    public SpawnPointChance wellPrioritization = SpawnPointChance.HighChance;
    [SerializeField] SpriteRenderer orbRenderer;
    [SerializeField] Animator orbAnimator;
    [SerializeField] Light2D orbLight;
    [SerializeField] GameObject mapIcon;
    private bool playerClose = false;
    public bool wellUsed {get; private set; } = false; 

    private ColorSlot activateOnSlot = null;

    void Start()
    {
        if(color == null) return;
        else Setup(color);
    }

    void OnEnable()
    {
        if(wellUsed) DisableWell();
    }

    public void Setup(GameColor color)
    {
        this.color = color;
        orbRenderer.material = color.colorMat;
        orbLight.color = color.lightTintColor;
        mapIcon.SetActive(true);

        if(wellUsed) 
        {
            orbAnimator.SetTrigger("deactivateWell");
            mapIcon.SetActive(false);
        }
    }

    void OnDisable()
    {
        playerClose = false;
    }

    public int GetColorAmount()
    {
        if(color != null && !wellUsed) return colorAmount;
        else return 0;
    }

    public void UseWellAnimation(ColorSlot colorSlot)
    {
        orbAnimator.SetBool("useWell", true);
        wellUsed = true;
        playerClose = false;
        activateOnSlot = colorSlot;
    }

    public void UseWell()
    {
        PlayerLevelMananger.instance.colorInventory.AddColor(color, colorAmount, activateOnSlot);
        PlayerLevelMananger.instance.playerCombatSystem.DeactivateAddColorMode();
        mapIcon.SetActive(false);
    }

    public void DisableWell()
    {
        orbRenderer.enabled = false;
        orbLight.gameObject.SetActive(false);
        mapIcon.SetActive(false);
        orbAnimator.SetTrigger("deactivateWell");
        wellUsed = true;
        
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
