using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameManager.instance.Pause();
        if (Player.instance)
        {
            Player.instance.playerUi.lightbox.SetActive(true);
            Player.instance.playerMovement.movementRoot.SetTotalRoot("bigMenuOpen", true);
        }
    }
}
