using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    //Button to start the navigation at when starting the game.
    [SerializeField] Button startButton;

    private void OnEnable() {
        startButton.Select();
    }

    //Load scene "Gem" when pressed.
    public void LoadScene()
    {
        SceneManager.LoadScene("Gem");
    }

    //Exit application when pressed.
    public void ExitGame() {
        Application.Quit();
        Debug.Log("Exit");
    }
}
