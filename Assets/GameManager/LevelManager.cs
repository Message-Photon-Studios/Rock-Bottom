using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] public string onDeathLevel;
    public bool saveProgressionOnStart = false;
    [SerializeField] string nextLevelName;
    [SerializeField] UIController canvas;
    public bool allowsClockTimer = true;
    [SerializeField] float addLevelClockTime;
    [SerializeField] bool restartClockTimer;
    [SerializeField] bool clearInventoryOnLevelEnd = false;
    [SerializeField] VideoPlayer videoOnPlayerDeath;
    [SerializeField] GameObject videoObjecCanvas;
    [SerializeField] GameObject backgroundMusic;
    [SerializeField] public bool allowTips = true;

    public static LevelManager instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

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
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            obj.GetComponent<PlayerLevelMananger>().SetStartLevel(this);
        }
        StartCoroutine(canvas.FadeOutCoroutine(true));
        if(GameManager.instance != null)
            GameManager.instance.disablePausing = false;

        GameManager.instance?.SetLevelManager(this, addLevelClockTime, restartClockTimer);
    }

    public IEnumerator EndLevelAsync()
    {
        SceneManager.LoadSceneAsync(nextLevelName);
        yield break;
    }

    public void EndLevel(string specialLevel)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(!specialLevel.Equals("")) nextLevelName = specialLevel;
        if (!clearInventoryOnLevelEnd)
        {
            GameManager.instance.disablePausing = true;
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
        GameManager.instance.disablePausing = true;
        backgroundMusic.SetActive(false);
        videoObjecCanvas.SetActive(true);
        videoOnPlayerDeath.Play();
        videoOnPlayerDeath.loopPointReached += DeathPlayerStopped;
    }



    public void ShowGame()
    {

    }
}
