using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralizes controll of opening and closing of any menu.
/// Any UIMenu or subclass will notify this when opened or closed.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public UIMenu currentlyOpen = null;
    [SerializeField] InputActionReference escapeHatch;
    [SerializeField] InputActionReference controllerBack;
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
        escapeHatch.action.performed += EscapeHatch;
        controllerBack.action.performed += ControllerBack;
    }

    void OnDestroy()
    {
        escapeHatch.action.performed -= EscapeHatch;
        controllerBack.action.performed -= ControllerBack;
    }

    /// <summary>
    /// When any menu is opened, notifies UIManager which acts as necessary.
    /// </summary>
    /// <param name="menu">Menu opened</param>
    public void MenuOpened(UIMenu menu)
    {
        if (currentlyOpen == menu) //If the same menu is already open, close it.
        {
            menu.CloseMenu();
            currentlyOpen = null;
        }
        else if (currentlyOpen != null) //If another menu is open, close that.
        {
            currentlyOpen.CloseMenu();
            currentlyOpen = menu;
        }
        else // If no menu is open.
        {
            currentlyOpen = menu;
        }
    }

    /// <summary>
    /// When any menu is closed, notifies UIManager.
    /// </summary>
    /// <param name="menu">Menu closed</param>
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
    /// <summary>
    /// En escape hatch that will get you out of any menu currently open.
    /// If no menu is open and the player exists, opens pause menu. 
    /// </summary>
    private void EscapeHatch()
    {
        if (currentlyOpen == null) //If no menu is open.
        {
            if (Player.instance != null) //If the player exists. 
            {
                Player.instance.playerUi.pauseMenu.OpenMenu();
            }
        }
        else if (Player.instance != null) //If a menu is open and the player exists.
        {
            if (Player.instance.playerUi.settings.mainComponent.activeSelf) //If the settings menu is open.
            {
                Player.instance.playerUi.pauseMenu.OpenMenu(); //Open pause menu.
            }
            else //If the settings menu is not open.
            {
                currentlyOpen.CloseMenu();
            }
        }
        else //If a menu is open but no player exists (Main menu).
        {
            currentlyOpen.CloseMenu();
        }
    }

    /// <summary>
    /// Called when the B or Circle button on a controller is pressed.
    /// If a big menu is currently open, make the B/Circle button act as an escape hatch.
    /// </summary>
    /// <param name="ctx"></param>
    private void ControllerBack(InputAction.CallbackContext ctx)
    {
        if (currentlyOpen && currentlyOpen.IsBigMenu())
        {
            EscapeHatch();
        }
    }
}
