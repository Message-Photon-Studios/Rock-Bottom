using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] string onDeathLevel;
    [SerializeField] string nextLevelName;
    [SerializeField] UIController canvas;
    public bool allowsClockTimer = true;
    [SerializeField] bool clearInventoryOnLevelEnd = false;
    [SerializeField] VideoPlayer videoOnPlayerDeath;
    [SerializeField] GameObject videoObjecCanvas;
    [SerializeField] GameObject backgroundMusic;

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
            GetComponent<ItemSpellManager>()?.SpawnItems();
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

        if (!clearInventoryOnLevelEnd)
        {
            canvas.disablePausing = true;
            if (player) player.GetComponent<Rigidbody2D>().simulated = false;
            if(player) player.GetComponent<Rigidbody2D>().velocity= Vector3.zero;
            player?.GetComponent<PlayerMovement>().movementRoot.SetTotalRoot("endLevel", true);
        } else
        {
            player?.GetComponent<PlayerStats>()?.onPlayerDied?.Invoke();
        }

        StartCoroutine(canvas.FadeOutCoroutine(false, EndLevelAsync));
    }

    public IEnumerator PlayerDiedAsync()
    {
        SceneManager.LoadSceneAsync(onDeathLevel);
        yield break;
    }

    public void PlayerDied()
    {
        if(videoOnPlayerDeath)
        {
            Time.timeScale = 0f;
            StartDeathVideo();
            
        } else
        {
            StartCoroutine(canvas.FadeOutCoroutine(false, PlayerDiedAsync));
        }
    }

    void DeathPlayerStopped(VideoPlayer vp)
    {   
        Time.timeScale = 1f;
        StartCoroutine(canvas.FadeOutCoroutine(false, PlayerDiedAsync));
    }

    void StartDeathVideo ()
    {
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().disablePausing = true;
        backgroundMusic.SetActive(false);
        videoObjecCanvas.SetActive(true);
        videoOnPlayerDeath.Play();
        videoOnPlayerDeath.loopPointReached += DeathPlayerStopped;
    }



    public void ShowGame()
    {

    }
}
