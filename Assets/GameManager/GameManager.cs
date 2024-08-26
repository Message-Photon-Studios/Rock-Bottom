using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] public float clockTime;
    [SerializeField] float hunterTime;
    [SerializeField] GameObject hunterPrefab;
    [SerializeField] float hunterSpawnDist;
    [SerializeField] int maxHunters;
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
        maxClockTime = clockTime;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
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
    }

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
    
    [System.Serializable]
    public struct MaskLibrary {
        public LayerMask onlyGround;
        public LayerMask onlyEnemy;
        public LayerMask onlyPlayer;
    }
}
