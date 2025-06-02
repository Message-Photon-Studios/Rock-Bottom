using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public UIMenu currentlyOpen = null;
    [SerializeField] InputActionReference escapeHatchInput;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            enabled = false;
        }
        escapeHatchInput.action.performed += EscapeHatch;
    }

    public void MenuOpened(UIMenu menu)
    {
        if (currentlyOpen == menu)
        {
            menu.CloseMenu();
            currentlyOpen = null;
        }
        else if (currentlyOpen != null)
        {
            currentlyOpen.CloseMenu();
            currentlyOpen = menu;
        }
        else
        {
            currentlyOpen = menu;
        }
    }

    public void MenuClosed(UIMenu menu)
    {
        if (currentlyOpen == menu)
        {
            currentlyOpen = null;
        }
    }

    private void EscapeHatch(InputAction.CallbackContext ctx)
    {
        EscapeHatch();
    }
    private void EscapeHatch()
    {
        if (currentlyOpen == null)
        {
            if (Player.instance != null)
            {
                Player.instance.playerUi.pauseMenu.OpenMenu();
            }
        }
        else
        {
            currentlyOpen.CloseMenu();
        }
        
    }
}
