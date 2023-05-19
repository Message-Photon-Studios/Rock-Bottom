using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    // [SerializedField] 

    public GameObject pauseMenuUI;
    public static bool GameIsPaused = false;

    // Start is called before the first frame update
    /*void Start()
    {
        pauseMenu.SetActive(false);

    }*/

    // Update is called once per frame

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Debug.Log("Ganme is paused...");

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Debug.Log("Game is resumed...");

    }

    

    public void GoToMainMenu()
    {
        // Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Loading to main menu...");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }
}