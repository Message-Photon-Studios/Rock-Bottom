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
    private static GameObject me;
    PlayerMovement movement; //Handles the players movement
    GameManager gameManager; //The game manager handles the specific scene
    Animator animator;
    PlayerStats stats; //A class that handles the players health and statistics

    private bool killMe = false;

    private void Awake()
    {
        //Singelton that ensures that only one player exists
        if(me && me != this.gameObject) 
        {
            foreach (GameObject obj in loadWithPlayerObjects)
            {
                Destroy(obj);
            }
            Destroy(this.gameObject);
            return;
        }
        else if(me == null) 
        {
            me = this.gameObject;
            foreach (GameObject obj in loadWithPlayerObjects)
            {
                DontDestroyOnLoad(obj);
            }
            DontDestroyOnLoad(this); 
            startPosition = transform.position;
        }

        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();

        stats.onPlayerDied += ForceKillPlayer;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Is called when a new level is loaded and generated
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetStartLevel(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        if(animator) animator.SetBool("dead", false);
        if(movement)
        {
            transform.position = startPosition;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().SetStartLevel();
            movement.movementRoot.SetTotalRoot("endLevel", false);
            movement.movementRoot.SetTotalRoot("dead", false);
        }
        GetComponent<Rigidbody2D>().simulated = true;
        
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
        me = null;
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