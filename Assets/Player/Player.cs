using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

/// <summary>
/// Handles player moving between scenes and player death
/// </summary>
public class Player : MonoBehaviour
{

    [Header("Control Inputs")]
    [SerializeField] InputActionReference   interactInput;
    [SerializeField] InputActionReference   attack1Input, attack2Input, attack3Input, attack4Input,
                                            removeColorInput1, removeColorInput2, removeColorInput3, removeColorInput4,
                                            rotateColorInput,
                                            walkInput, jumpInput, verticalMoveInput, dashInput, lookInput;


    [Header("Load with player")]
    [SerializeField] GameObject[] loadWithPlayerObjects; //Objects that will be set as dont destroy on load with the player and then destroyed at the same time
    Vector3 startPosition;
    private bool killMe = false;
    public static Player instance;

    #region  Player Managers & Systems
    public PlayerMovement playerMovement {get; private set;} //Handles the players movement
    public LevelManager levelManager {get; private set;} //The game manager handles the specific scene
    public PlayerStats playerStats {get; private set;}
    public PlayerCombatSystem playerCombatSystem {get; private set;}
    public ItemInventory playerInventory {get; private set;}
    public ColorInventory colorInventory {get; private set;}
    public UIController playerUi {get; private set;}
    Animator animator;
    public PlayerStats stats {get; private set;} //A class that handles the players health and statistics
    #endregion

    #region Input setup
    public Action   interactAction;
    public Action <int> attackAction, removeColorAction;
    public Action<int> rotateColorAction;
    public Action jumpAction, jumpCancelAction, dashAction;
    public Action<float> lookAction;
    public Action lookCancelAction; 
    public Action verticalMoveAction, verticalMoveCancelAction;

    public float walkDir = 0;
    public float verticalMoveDir = 0;

    private void SetupInputs ()
    {
        interactInput.action.performed += (InputAction.CallbackContext ctx) => interactAction?.Invoke();
        
        attack1Input.action.performed += (InputAction.CallbackContext ctx) => attackAction?.Invoke(0);
        attack2Input.action.performed += (InputAction.CallbackContext ctx) => attackAction?.Invoke(1);
        attack3Input.action.performed += (InputAction.CallbackContext ctx) => attackAction?.Invoke(2);
        attack4Input.action.performed += (InputAction.CallbackContext ctx) => attackAction?.Invoke(3);
        removeColorInput1.action.performed += (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(0);
        removeColorInput2.action.performed += (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(1);
        removeColorInput3.action.performed += (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(2);
        removeColorInput4.action.performed += (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(3);
        rotateColorInput.action.performed += (InputAction.CallbackContext ctx) => rotateColorAction?.Invoke(rotateColorInput.action.ReadValue<int>());

        jumpInput.action.performed += (InputAction.CallbackContext ctx) => jumpAction?.Invoke();
        jumpInput.action.canceled += (InputAction.CallbackContext ctx) => jumpCancelAction?.Invoke();

        dashInput.action.performed += (InputAction.CallbackContext ctx) => dashAction?.Invoke();

        lookInput.action.performed += (InputAction.CallbackContext ctx) => lookAction?.Invoke(lookInput.action.ReadValue<float>());
        lookInput.action.canceled += (InputAction.CallbackContext ctx) => lookCancelAction?.Invoke();

        walkInput.action.performed += (InputAction.CallbackContext ctx) => {walkDir = walkInput.action.ReadValue<float>();};
        walkInput.action.canceled += (InputAction.CallbackContext ctx) => {walkDir = walkInput.action.ReadValue<float>();};

        verticalMoveInput.action.performed += (InputAction.CallbackContext ctx) => {
            verticalMoveDir = verticalMoveInput.action.ReadValue<float>(); 
            verticalMoveAction?.Invoke();
            };
        verticalMoveInput.action.canceled += (InputAction.CallbackContext ctx) => {
            verticalMoveDir = verticalMoveInput.action.ReadValue<float>();
            verticalMoveCancelAction?.Invoke();
            };
    }
    
    public void RemoveActionListeners()
    {
        interactInput.action.performed -= (InputAction.CallbackContext ctx) => interactAction?.Invoke();
        
        attack1Input.action.performed -= (InputAction.CallbackContext ctx) => attackAction?.Invoke(0);
        attack2Input.action.performed -= (InputAction.CallbackContext ctx) => attackAction?.Invoke(1);
        attack3Input.action.performed -= (InputAction.CallbackContext ctx) => attackAction?.Invoke(2);
        attack4Input.action.performed -= (InputAction.CallbackContext ctx) => attackAction?.Invoke(3);
        removeColorInput1.action.performed -= (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(0);
        removeColorInput2.action.performed -= (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(1);
        removeColorInput3.action.performed -= (InputAction.CallbackContext ctx) => removeColorAction?.Invoke(2);
        rotateColorInput.action.performed -= (InputAction.CallbackContext ctx) => rotateColorAction?.Invoke(rotateColorInput.action.ReadValue<int>());

        jumpInput.action.performed -= (InputAction.CallbackContext ctx) => jumpAction?.Invoke();
        dashInput.action.performed -= (InputAction.CallbackContext ctx) => dashAction?.Invoke();

        lookInput.action.performed -= (InputAction.CallbackContext ctx) => lookAction?.Invoke(lookInput.action.ReadValue<float>());
        lookInput.action.canceled -= (InputAction.CallbackContext ctx) => lookCancelAction?.Invoke();

        walkInput.action.performed -= (InputAction.CallbackContext ctx) => {walkDir = walkInput.action.ReadValue<float>();};
        walkInput.action.canceled -= (InputAction.CallbackContext ctx) => {walkDir = walkInput.action.ReadValue<float>();};

        verticalMoveInput.action.performed -= (InputAction.CallbackContext ctx) => {
            verticalMoveDir = verticalMoveInput.action.ReadValue<float>(); 
            verticalMoveAction?.Invoke();
            };
        verticalMoveInput.action.canceled -= (InputAction.CallbackContext ctx) => {
            verticalMoveDir = verticalMoveInput.action.ReadValue<float>();
            verticalMoveCancelAction?.Invoke();
            };
    }

    #endregion

    #region Initialization and Level loading


    private void Awake()
    {
        Debug.Log("---------------- Start player init -------------------");
        Debug.Log("This object ID: " + gameObject.GetInstanceID());
        Debug.Log("Har previous instance: " + (instance != null));
        if(instance != null) Debug.Log("Previous instance ID: " + instance.gameObject.GetInstanceID());
        if(instance != null) Debug.Log("Destroy previous instance: " + instance.killMe);

        if(instance != null && instance.killMe && instance != this)
        {
            instance.DestroyPlayer();
            instance = null;
        }

        //Singelton that ensures that only one player exists
        if(instance && instance != this.gameObject) 
        {
            instance.transform.position = this.gameObject.transform.position;
            instance.GetComponent<Player>().SetStartPosition(transform.position);

            Debug.Log("Set new start position " + startPosition);
            DestroyPlayer();
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

        SetupInputs();

        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<ItemInventory>();
        playerCombatSystem = GetComponent<PlayerCombatSystem>();
        colorInventory = GetComponent<ColorInventory>();

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") DestroyPlayer();
    }

    #endregion
    
    #region Update Loop

    void Update()
    {
        
    }

    #endregion
    
    #region Player destruction and Force kill
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
        killMe = true;
    }

    private void OnDestroy()
    {   
        Debug.Log("Player destroyed. ID: " + GetInstanceID());
    }

    public void DestroyPlayer()
    {
        Debug.Log("Start destroying player with ID: " + GetInstanceID());
        RemoveActionListeners();
        if(stats)
            stats.onPlayerDied -= ForceKillPlayer;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        foreach (GameObject obj in loadWithPlayerObjects)
            {
                Destroy(obj);
            }
        Destroy(gameObject);
    }

    #endregion
}
