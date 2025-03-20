using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;

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

    [Header("Color Wells")]
    [SerializeField] int wellSpawnAmount = 2;
    [SerializeField] GameColor[] spawnableColors;
    [SerializeField] int sameColorRerolls = 1;
    private HashSet<GameColor> spawnedColors = new HashSet<GameColor>();

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

        spawnedColors = new HashSet<GameColor>();
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
            obj.GetComponent<Player>().SetStartLevel(this);
        }
        StartCoroutine(canvas.FadeOutCoroutine(true));
        if(GameManager.instance != null)
            GameManager.instance.disablePausing = false;

        GameManager.instance?.SetLevelManager(this, addLevelClockTime, restartClockTimer);
        ProneColorWells();
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

    private void ProneColorWells()
    {
        ColorWell[] colorWells = FindObjectsOfType<ColorWell>();
        List<ColorWell> prioritizedWells = new List<ColorWell>();
        List<ColorWell> backupWells = new List<ColorWell>();

        for (int i = 0; i < colorWells.Length; i++)
        {
            if(colorWells[i].wellPrioritization == SpawnPointChance.Guaranteed)
            {
                if(colorWells[i].color == null) colorWells[i].Setup(GetColorWellColor());
                continue;
            }

            colorWells[i].gameObject.SetActive(false);
            if(colorWells[i].wellPrioritization == SpawnPointChance.HighChance) prioritizedWells.Add(colorWells[i]);
            else backupWells.Add(colorWells[i]);
        }
        int spawned = 0;
        for (int i = 0; i < wellSpawnAmount && prioritizedWells.Count > 0; i++)
        {
            int picker = Random.Range(0, prioritizedWells.Count);
            ColorWell well = prioritizedWells[picker];
            prioritizedWells.RemoveAt(picker);
            well.gameObject.SetActive(true);
            if(well.color == null) well.Setup(GetColorWellColor());
            spawned ++;
        }

        if(spawned >= wellSpawnAmount) return;

        for (int i = 0; i < wellSpawnAmount && spawned < wellSpawnAmount && backupWells.Count > 0; i++)
        {
            int picker = Random.Range(0, backupWells.Count);
            ColorWell well = backupWells[picker];
            backupWells.RemoveAt(picker);
            well.gameObject.SetActive(true);
            if(well.color == null) well.Setup(GetColorWellColor());
            spawned ++;
        }
    }

    public GameColor GetColorWellColor()
    {
        if(spawnedColors.Count >= spawnableColors.Length) spawnedColors = new HashSet<GameColor>();

        GameColor color = spawnableColors[Random.Range(0, spawnableColors.Length)];
        for (int i = 0; i < sameColorRerolls; i++)
        {
            if(spawnedColors.Contains(color)) color = spawnableColors[Random.Range(0, spawnableColors.Length)];
            else break;
        }

        if(!spawnedColors.Contains(color)) spawnedColors.Add(color);
        return color;
    }
}
