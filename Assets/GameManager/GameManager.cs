using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

    string gameStartScene = "Tutorial";
    float hunterTimer = 0f;
    int hunters = 0;
    float maxClockTime;
    public MaskLibrary maskLibrary;
    private PlayerStats player;

    private LevelManager currentLevelManager;

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
    }

    void Start()
    {
        unlockedSpells = new List<string>();
        spawnableSpells = new HashSet<string>();
        foreach (ColorSpell spell in startSpells)
        {
            spawnableSpells.Add(spell.name);
        }

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
    }

    public void SetLevelManager (LevelManager levelManager, float addClockTime, bool restartTimer) 
    {
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
    }

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
    }

    void IDataPersistence.SaveData(GameData data)
    {
        data.startScene = gameStartScene;
        data.unlockedColorSpells = unlockedSpells.ToArray();
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

        DataPersistenceManager.instance.SaveGame();
    }

    #endregion
    
    #region Mask Library
    [System.Serializable]
    public struct MaskLibrary {
        public int playerLayer;
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
