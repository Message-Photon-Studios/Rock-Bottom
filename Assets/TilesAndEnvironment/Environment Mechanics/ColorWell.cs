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
    private Color iconShadedColor = Color.black;
    private bool playerClose = false;
    public bool wellUsed {get; private set; } = false; 

    private ColorSlot activateOnSlot = null;

    void Start()
    {
        if(iconShadedColor == Color.black) iconShadedColor = colorIconImage.color;
        if(color == null) return;
        else Setup(color);
    }

    void OnEnable()
    {
        if(iconShadedColor == Color.black) iconShadedColor = colorIconImage.color;
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
        orbRenderer.enabled = true;
        orbLight.enabled = true;
        mapIcon.GetComponent<SpriteRenderer>().sprite = color.colorIcon;
        mapIcon.SetActive(true);
        colorIconImage.sprite = color.colorIcon;
        //colorIconImage.material = color.colorMat;

        if(wellUsed) 
        {
            DisableWell();
        } else orbAnimator.SetBool("useWell", false);
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

    public void AddColorAmount(int addAmount)
    {
        colorAmount += addAmount;
        if(colorAmount > 0) 
        {
            colorIconImage.color = Color.white;
            wellUsed = false;
            Setup(color);
            Player.instance.playerCombatSystem.EnableAbsorbColor(this);
        }
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
        Player.instance.colorInventory.AddColor(color, colorAmount, activateOnSlot);
        colorAmount = 0;
        mapIcon.SetActive(false);
        colorIconImage.color = iconShadedColor;
    }

    public void DisableWell()
    {
        orbRenderer.enabled = false;
        orbLight.gameObject.SetActive(false);
        mapIcon.SetActive(false);
        orbAnimator.SetTrigger("deactivateWell");
        colorIconImage.color = iconShadedColor;
        Player.instance.playerCombatSystem.DeactivateAddColorMode();
        wellUsed = true;
        
    }
    
    #region Check playerClose
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = true;
            colorIconImage.gameObject.SetActive(true);
            if(!wellUsed) 
            {
                colorIconImage.color = Color.white;
            }
            Player.instance.playerCombatSystem.EnableAbsorbColor(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerClose = false;
            colorIconImage.color = iconShadedColor;
            if(wellUsed) colorIconImage.gameObject.SetActive(false);
            Player.instance.playerCombatSystem.DisableAbsorbColor();
            Player.instance.playerCombatSystem.MovedAwayFromWell(this);
        }
    }
    #endregion
}
