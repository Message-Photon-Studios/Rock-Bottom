using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor.UI;
using UnityEngine.UI;

public class ColorWell : MonoBehaviour
{
    public GameColor color;
    [SerializeField] int colorAmount;
    
    public SpawnPointChance wellPrioritization = SpawnPointChance.HighChance;
    [SerializeField] SpriteRenderer orbRenderer;
    [SerializeField] Animator orbAnimator;
    [SerializeField] Light2D orbLight;
    [SerializeField] GameObject mapIcon;
    [SerializeField] SpriteRenderer colorIconImage;
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
        else
        {
            mapIcon.SetActive(true);
        }
    }

    public void Setup(GameColor color)
    {
        this.color = color;
        orbRenderer.material = color.colorMat;
        orbLight.color = color.lightTintColor;
        mapIcon.GetComponent<SpriteRenderer>().sprite = color.colorIcon;
        mapIcon.SetActive(true);
        colorIconImage.sprite = color.colorIcon;
        //colorIconImage.material = color.colorMat;
        colorIconImage.gameObject.SetActive(false);

        if(wellUsed) 
        {
            DisableWell();
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
        colorIconImage.gameObject.SetActive(false);
    }

    public void DisableWell()
    {
        orbRenderer.enabled = false;
        orbLight.gameObject.SetActive(false);
        mapIcon.SetActive(false);
        colorIconImage.gameObject.SetActive(false);
        orbAnimator.SetTrigger("deactivateWell");
        wellUsed = true;
        
    }
    
    #region Check playerClose
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = true;
            if(!wellUsed) colorIconImage.gameObject.SetActive(true);
            PlayerLevelMananger.instance.playerCombatSystem.EnableAbsorbColor(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = false;
            colorIconImage.gameObject.SetActive(false);
            PlayerLevelMananger.instance.playerCombatSystem.DisableAbsorbColor();
        }
    }
    #endregion
}
