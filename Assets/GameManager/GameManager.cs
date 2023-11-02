using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] string onDeathLevel;
    [SerializeField] string nextLevelName;
    [SerializeField] UIController canvas;
     public bool allowsClockTimer = true;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        if (levelGenerator)
        {
            levelGenerator.init(canvas, true);
        }
        else
        {
            canvas.loaded = true;
            FinishedGeneration();
        }
    }
    
    public void FinishedGeneration()
    {
        Debug.Log("Test");
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            obj.GetComponent<PlayerLevelMananger>().SetStartLevel(this);
        }
        StartCoroutine(canvas.FadeOutCoroutine(true));
        canvas.disablePausing = false;
    }

    public IEnumerator EndLevelAsync()
    {
        SceneManager.LoadSceneAsync(nextLevelName);
        yield break;
    }

    public void EndLevel()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        canvas.disablePausing = true;
        if(player) player.GetComponent<Rigidbody2D>().simulated = false;
        player?.GetComponent<PlayerMovement>().movementRoot.SetTotalRoot("endLevel", true);
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
