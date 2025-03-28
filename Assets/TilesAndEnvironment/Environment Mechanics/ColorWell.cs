using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorWell : InteractionObject
{
    public GameColor color;
    [SerializeField] int colorAmount;
    
    public SpawnPointChance wellPrioritization = SpawnPointChance.HighChance;
    [SerializeField] SpriteRenderer orbRenderer;
    [SerializeField] Animator orbAnimator;
    [SerializeField] Light2D orbLight;
    [SerializeField] GameObject mapIcon;
    [SerializeField] SpriteRenderer colorIconImage;
    [SerializeField] PickUpCanvasController pickUpCanvasController;
    private Color iconShadedColor = Color.black;
    public bool wellUsed {get; private set; } = false; 

    private ColorSlot activateOnSlot = null;

    protected override void Start()
    {
        base.Start();
        
        if(colorIconImage == null) return;
        if(iconShadedColor == Color.black) iconShadedColor = colorIconImage.color;
        if(color == null) return;
        else Setup(color);
    }

    void OnEnable()
    {
        if(colorIconImage == null) return;
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
        } else orbAnimator.SetBool("useWell", false);
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

    protected override void PlayerInteract()
    {
        if(!Player.instance.playerCombatSystem.addColorMode)
        {
            Player.instance.playerCombatSystem.EnableAbsorbColor(this);
        } else
        {
            Player.instance.playerCombatSystem.DeactivateAddColorMode();
        }

        pickUpCanvasController.SetColorShrine(this);
    }

    #region Check playerClose
    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);

        if(isClose)
        {
            colorIconImage.gameObject.SetActive(true);
            if(!wellUsed) 
            {
                colorIconImage.color = Color.white;
            }
        
            pickUpCanvasController.SetColorShrine(this);
        } else
        {
            pickUpCanvasController.CloseUi();
            colorIconImage.color = iconShadedColor;
            if(wellUsed) colorIconImage.gameObject.SetActive(false);
            Player.instance.playerCombatSystem.DeactivateAddColorMode();
            Player.instance.playerCombatSystem.MovedAwayFromWell(this);
        }
    }
    #endregion
}
