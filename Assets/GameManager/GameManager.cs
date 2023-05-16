using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] string onDeathLevel;
    [SerializeField] string nextLevelName;
    [SerializeField] string mainMenuName;
    [SerializeField] InputActionReference menuAction;
    [SerializeField] UIController canvas;
    
    private void Start()
    {
        if (levelGenerator)
            levelGenerator.init(canvas);
        else
            canvas.loaded = true;
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
    public IEnumerator EndLevelAsync()
    {
        SceneManager.LoadSceneAsync(nextLevelName);
        yield break;
    }

    public void EndLevel()
    {
        StartCoroutine(canvas.FadeOutCoroutine(false, EndLevelAsync));
    }

    public IEnumerator PlayerDiedAsync()
    {
        SceneManager.LoadSceneAsync(onDeathLevel);
        yield break;
    }

    public void PlayerDied()
    {
        StartCoroutine(canvas.FadeOutCoroutine(false, PlayerDiedAsync));
    }

    public void ShowGame()
    {

    }
}
