using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;
    
    [SerializeField] public float clockTime;
    [SerializeField] float hunterTime;
    [SerializeField] GameObject hunterPrefab;
    [SerializeField] float hunterSpawnDist;
    [SerializeField] int maxHunters;
    
    [SerializeField] ColorSpell[] startSpells;
    private List<string> unlockedSpells;
    private HashSet<string> spawnableSpells;
    [SerializeField] AudioSource spellUnlockSound;

    [SerializeField] AudioSource petrifiedPickupSound;
    public SoundEffectManager soundEffectManager;

    [SerializeField] private int petrifiedPigment = 0;
    [SerializeField] private int inspirationPoints = 0;
    string gameStartScene = "Tutorial_0";
    public bool allowsTips {get; private set;} = true;
    float hunterTimer = 0f;
    int hunters = 0;
    float maxClockTime;
    public MaskLibrary maskLibrary;
    private PlayerStats player;
    private UIController uiController;
    private LevelManager currentLevelManager;
    public UnityAction onLevelLoaded;

    public TipsManager tipsManager;

    public ItemLibrary itemLibrary;

    private bool prepareStartRun = false;

    /// <summary>
    /// Is called right before a new run is loaded. This is called when the player is exiting cave town.
    /// </summary>
    public System.Action onPrepareNewRun;

    /// <summary>
    /// Is called right after a new run is loaded. This is called in the first level.
    /// </summary>
    public System.Action onStartedNewRun;

    /// <summary>
    /// Is called after the player has died and cave town is loaded.
    /// </summary>
    public System.Action onLoadedCaveTown;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }

        spawnableSpells = new HashSet<string>();
        foreach (ColorSpell spell in startSpells)
        {
            spawnableSpells.Add(spell.name);
        }
    }

    void Start()
    {
        unlockedSpells = new List<string>();

        maxClockTime = clockTime;
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        if(player != null)
            player.onPlayerDied += OnPlayerDied;
        
    }

    private void OnPlayerDied()
    {
        clockTime = maxClockTime;
        hunterTimer = 0;
        hunters = 0;
        DataPersistenceManager.instance.SaveGame();
    }

    public void SetUiController(UIController uiController)
    {
        this.uiController = uiController;
        tipsManager.SetUi(uiController);
    }

    public void SetLevelManager (LevelManager levelManager, float addClockTime, bool restartTimer) 
    {
        DataPersistenceManager.instance.SaveGame();
        DataPersistenceManager.instance.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        currentLevelManager = levelManager;
        hunterTimer = 0f;
        hunters = 0;
        if(!restartTimer)
            clockTime = addClockTime + ((clockTime < 0)?0:clockTime);
        else
            clockTime = maxClockTime;

        if(levelManager.saveProgressionOnStart)
        {
            gameStartScene = levelManager.onDeathLevel;
            DataPersistenceManager.instance.SaveGame();
        }
        
        allowsTips = levelManager.allowTips;
        onLevelLoaded?.Invoke();
        if(prepareStartRun)
        {
            prepareStartRun = false;
            onStartedNewRun?.Invoke();
        } else if(levelManager.isCaveTownLevel)
        {
            DataPersistenceManager.instance.LoadGame();
            onLoadedCaveTown?.Invoke();
        }
    }

    public void SetStartRun()
    {
        prepareStartRun = true;
        onPrepareNewRun?.Invoke();
    }

    #region MainMenu and Quit
    public void GoToMainMenu()
    {
        StartCoroutine(uiController.FadeOutCoroutine(false, GoToMainMenuAsync));
        Resume();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DataPersistenceManager.instance.SaveGame();
        Debug.Log("Loading to main menu...");
    }
    public IEnumerator GoToMainMenuAsync()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        yield break;
    }

    public void QuitGame()
    {
        DataPersistenceManager.instance.SaveGame();
        Application.Quit();
        Debug.Log("Quitting game...");
    }

    #endregion

    #region Save Load

    void IDataPersistence.LoadData(GameData data)
    {
        gameStartScene = data.startScene;
        spawnableSpells = new HashSet<string>();
        unlockedSpells = new List<string>();
        foreach (ColorSpell spell in startSpells)
        {
            spawnableSpells.Add(spell.name);
        }
        spawnableSpells.AddRange(data.unlockedColorSpells);
        unlockedSpells.AddRange(data.unlockedColorSpells);

        petrifiedPigment = data.petrifiedPigment;
        pickedUpPetrifiedPigment = new HashSet<string>();
        pickedUpPetrifiedPigment.AddRange(data.petrifiedPigmentPickedUp);

        inspirationPoints = data.inspirationPoints;
    }

    void IDataPersistence.SaveData(GameData data)
    {
        data.startScene = gameStartScene;
        data.unlockedColorSpells = unlockedSpells.ToArray();
        data.petrifiedPigment = petrifiedPigment;
        data.petrifiedPigmentPickedUp = pickedUpPetrifiedPigment.ToArray();
        data.inspirationPoints = inspirationPoints;
    }

    public string GetStartScene()
    {
        return gameStartScene;
    }

    #endregion

    #region Game clock

    void Update()
    {
        if (currentLevelManager && currentLevelManager.allowsClockTimer)
        {
            clockTime -= Time.deltaTime;

            if (clockTime <= 0)
            {
                hunterTimer -= Time.deltaTime;
                if (hunterTimer <= 0 && hunters < maxHunters)
                {
                    SpawnHunter();
                    hunterTimer = hunterTime;
                }
            }
        }
    }

    void SpawnHunter()
    {
        GameObject hunter = GameObject.Instantiate(hunterPrefab, player.transform.position + (new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f),0).normalized*hunterSpawnDist), hunterPrefab.transform.rotation,GameObject.Find("EnemyHolder").transform);
        Debug.Log("Hunter spawned");
        hunters++;
    }

    public void RespawnHunter()
    {
        GameObject hunter = GameObject.Instantiate(hunterPrefab, player.transform.position + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * hunterSpawnDist), hunterPrefab.transform.rotation, GameObject.Find("EnemyHolder").transform);
        Debug.Log("Hunter ReSpawned");
    }

    /// <summary>
    /// Returns the clock time as a formated string in the format "min:sec"
    /// </summary>
    /// <returns></returns>
    public (string, float, Color)  GetClockTimeString()
    {
        string retString;
        float retSize;
        Color retColor;

        if(clockTime > 0)
        {
            int min = (int)clockTime/ 60;
            int sec = (int)clockTime % 60;
            retString =  ((min < 0) ? "00" : (min < 10) ? "0" + min : min) + ":" + ((sec<0)?"00":(sec<10)?"0"+sec:sec);
            retColor = Color.white;
            retSize = 180;
        }
        else 
        {
            retString = hunters + ((hunters > 1)?"x Hunters":"x Hunter");
            retColor = Color.red;
            retSize = (hunters > 1)?355:310;
        } 

        return (retString, retSize, retColor);
    }

    public (int, int) getTime()
    {
        if(clockTime > 0)
        {
            return ((int)clockTime / 60, (int)clockTime % 60);
        }

        return (0, 0);
    }

    #endregion

    #region Spell Unlock

    /// <summary>
    /// Checks if this spell is spawnable
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    public bool IsSpellSpawnable(ColorSpell spell)
    {
        bool ret = spawnableSpells.Contains(spell.name);
        Debug.Log(spell.name + " is spawnable: " + ret);

        return ret;
    }


    /// <summary>
    /// Permanently unlocks this spell.
    /// </summary>
    /// <param name="spell"></param>
    public void UnlockedSpell(ColorSpell spell)
    {
        unlockedSpells.Add(spell.name);
        spawnableSpells.Add(spell.name);
        spellUnlockSound.Play();
        DataPersistenceManager.instance.SaveGame();
    }

    #endregion

    #region Petrified Pigment

    public UnityAction<int> onPetrifiedPigmentChanged;

    public bool TryRemovePetrifiedPigment(int removePigment)
    {
        if(removePigment > petrifiedPigment) return false;

        petrifiedPigment-=removePigment;
        onPetrifiedPigmentChanged?.Invoke(-removePigment);
        return true;
    }

    public bool HasPetrifiedPigment(int hasAmount)
    {
        return petrifiedPigment >= hasAmount;
    }

    public int GetPetrifiedPigmentAmount()
    {
        return petrifiedPigment;
    }

    HashSet<string> pickedUpPetrifiedPigment = new HashSet<string>();

    public bool IsPetrifiedPigmentPickedUp(string name)
    {
        return pickedUpPetrifiedPigment.Contains(name);
    }

    public void PickedUpPetrifiedPigment(string name)
    {
        petrifiedPigment ++;
        pickedUpPetrifiedPigment.Add(name);
        onPetrifiedPigmentChanged?.Invoke(1);
        petrifiedPickupSound.Play();
        DataPersistenceManager.instance.SaveGame();
    }

    #endregion

    #region Inspiration Points

    public UnityAction<int> onInspirationChanged;

    public int GetInspiration ()
    {
        return inspirationPoints;
    }

    public void AddInspiration(int addInspiration)
    {
        inspirationPoints += addInspiration;
        onInspirationChanged?.Invoke(addInspiration);
        DataPersistenceManager.instance.SaveGame();
    }

    #endregion
    
    #region Pause Game
    [HideInInspector] public bool disablePausing = false;

    /// <summary>
    /// Pauses the game by setting timescale to 0.
    /// </summary>
    public void Pause()
    {
        if(disablePausing) return;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Game is paused...");

    }

    /// <summary>
    /// Resumes the game by setting timescale to 1.
    /// </summary>
    public void Resume()
    {
        if (Time.timeScale == 1f) return; 
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Game is resumed...");
    }
    #endregion

    #region Mask Library
    [System.Serializable]
    public struct MaskLibrary {
        public int playerLayer;
        public int playerFeetLayer;
        public int platformLayer;
        public int groundLayer;
        public LayerMask onlyGround;
        public LayerMask onlyPlatforms;
        public LayerMask onlyEnemy;
        public LayerMask onlyPlayer;

        public LayerMask onlySolidGround()
        {
            return onlyGround & ~onlyPlatforms;
        }
    }

    #endregion
}
