using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    [SerializeField] Button resumeButton;

    [SerializeField] UIController uiController;

    private void OnEnable() {
        eventSystem.SetSelectedGameObject(null);
        resumeButton.GetComponent<Selectable>().Select();
    }

    public void ResumeButton(){
        uiController.OpenPauseMenu();
    }

    public void OpenMap() {
        uiController.OpenMap();
    }

    public void OpenInventory() {
        uiController.OpenInventory();
    }

    public void GoToMainMenu()
    {
        GameManager.instance.GoToMainMenu();
    }


    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}