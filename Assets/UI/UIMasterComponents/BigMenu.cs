using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A big menu will pause the game and root the player as well as turning on a lightbox.
/// It does this after opening or closing the menu. 
/// </summary>
public class BigMenu : UIMenu
{
    protected override void AfterClosing()
    {
        GameManager.instance.Resume();
        if (Player.instance)
        {
            Player.instance.playerUi.lightbox.SetActive(false);
            Player.instance.playerMovement.movementRoot.SetTotalRoot("bigMenuOpen", false);
        }
    }

    protected override void AfterOpening()
    {
        if (UIManager.instance.currentlyOpen != null)
        {
            GameManager.instance.Pause();
            if (Player.instance)
            {
                Player.instance.playerUi.lightbox.SetActive(true);
                Player.instance.playerMovement.movementRoot.SetTotalRoot("bigMenuOpen", true);
            }
        }
    }

    public override bool IsBigMenu()
    {
        return true;
    }
}
