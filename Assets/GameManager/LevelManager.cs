using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using Unity.VisualScripting;

[RequireComponent(typeof (EnemyManager))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;

    [Header("Adjacent Levels")]
    [SerializeField] public string onDeathLevel;
    [SerializeField] string nextLevelName;


    [Header("Clock Timer")]
    public bool allowsClockTimer = true;
    [SerializeField] float addLevelClockTime;
    [SerializeField] bool restartClockTimer;

    [Header("Settings")]
    public bool saveProgressionOnStart = false;
    [SerializeField] bool clearInventoryOnLevelEnd = false;
    [SerializeField] public bool allowTips = true;
    [SerializeField] public bool isCaveTownLevel = false;

    [Header("References")]
    [SerializeField] UIController canvas;
    [SerializeField] GameObject backgroundMusic;
    
    [Header("Video on Death")]
    [SerializeField] VideoPlayer videoOnPlayerDeath;
    [SerializeField] GameObject videoObjecCanvas;

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
        DataPersistenceManager.instance.Start();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        if (levelGenerator)
        {
            ItemSpellManager.instance.ClearPetrifiedPigmentList();
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

    public EnemyManager GetEnemyManager()
    {
        return GetComponent<EnemyManager>();
    }
}
