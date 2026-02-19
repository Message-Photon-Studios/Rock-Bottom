using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the logic of the pause menu.
/// </summary>
public class PauseMenu : BigMenu
{
    [SerializeField] EventSystem eventSystem;

    [SerializeField] Button resumeButton;

    [SerializeField] UIController uiController;

    public void ResumeButton()
    {
        CloseMenu();
    }

    public void OpenMap()
    {
        uiController.map.OpenMenu();
    }

    public void OpenInventory()
    {
        uiController.inventory.OpenMenu();
    }

    public void OpenSettings()
    {
        uiController.settings.OpenMenu();
    }

    public void GoToMainMenu()
    {
        GameManager.instance.GoToMainMenu();
    }


    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    protected override void AfterOpening()
    {
        base.AfterOpening();
        eventSystem.SetSelectedGameObject(null);
        resumeButton.GetComponent<Selectable>().Select();
    }
    
}