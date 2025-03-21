using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player moving between scenes and player death
/// </summary>
public class PlayerLevelMananger : MonoBehaviour
{
    [SerializeField] GameObject[] loadWithPlayerObjects; //Objects that will be set as dont destroy on load with the player and then destroyed at the same time
    Vector3 startPosition;
    public static PlayerLevelMananger instance;
    public PlayerMovement playerMovement {get; private set;} //Handles the players movement
    public LevelManager levelManager {get; private set;} //The game manager handles the specific scene
    public PlayerStats playerStats {get; private set;}
    public ItemInventory playerInventory {get; private set;}
    public UIController playerUi {get; private set;}
    Animator animator;
    PlayerStats stats; //A class that handles the players health and statistics

    private bool killMe = false;

    private void Awake()
    {
        //Singelton that ensures that only one player exists
        if(instance && instance != this.gameObject) 
        {
            instance.transform.position = this.gameObject.transform.position;
            instance.GetComponent<PlayerLevelMananger>().SetStartPosition(transform.position);

            Debug.Log("Set new start position " + startPosition);

            foreach (GameObject obj in loadWithPlayerObjects)
            {
                Destroy(obj);
            }
            Destroy(this.gameObject);
            return;
        }
        else if(instance == null) 
        {
            instance = this;
            foreach (GameObject obj in loadWithPlayerObjects)
            {
                if(obj != null)
                    DontDestroyOnLoad(obj);
            }
            DontDestroyOnLoad(this); 

            if (startPosition == Vector3.zero) 
            {
                startPosition = this.transform.position;
                Debug.Log("Set original start position " + startPosition );
            }
        }

        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<ItemInventory>();

        foreach (GameObject obj in loadWithPlayerObjects)
        {
            if(obj.GetComponent<UIController>())
            {
                playerUi = obj.GetComponent<UIController>();
                break;
            }
        }

        stats.onPlayerDied += ForceKillPlayer;
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("Finished setup");
    }

    public void SetStartPosition(Vector3 newStartPosition)
    {
        startPosition = newStartPosition;
    }

    /// <summary>
    /// Is called when a new level is loaded and generated
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetStartLevel(LevelManager gameManager)
    {
        this.levelManager = gameManager;
        
        if(animator) animator.SetBool("dead", false);
        if(playerMovement)
        {
            transform.position = startPosition;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().SetStartLevel();
            playerMovement.movementRoot.SetTotalRoot("endLevel", false);
            playerMovement.movementRoot.SetTotalRoot("dead", false);
        }
        GetComponent<Rigidbody2D>().simulated = true;

        foreach(GameObject obj in loadWithPlayerObjects)
        {
            obj.transform.position = new Vector3(startPosition.x, startPosition.y, obj.transform.position.z);
        }
        
        if(stats) stats.Setup(gameManager);
    }

    /// <summary>
    /// Is called when the player is supposed to die when loading the next scene
    /// </summary>
    public void ForceKillPlayer()
    {
        gameObject.tag = "DeadPlayer";
        foreach (GameObject obj in loadWithPlayerObjects)
        {
            obj.tag = "DeadPlayer";
        }
        instance = null;
        killMe = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!killMe && scene.name != "MainMenu") return;

        foreach (GameObject obj in loadWithPlayerObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(stats)
            stats.onPlayerDied -= ForceKillPlayer;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
