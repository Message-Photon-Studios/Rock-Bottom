using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] string nextLevelName;
    [SerializeField] string mainMenuName;
    [SerializeField] InputActionReference menuAction;
    private void Start() 
    {
        levelGenerator.init();
    }

    void OnEnable()
    {
        menuAction.action.performed += LoadMainMenu;
    }

    void OnDisable()
    {
        menuAction.action.performed -= LoadMainMenu;
    }

    private void LoadMainMenu(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(mainMenuName);
    }

    public void EndLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
