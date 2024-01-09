using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    private ColorInventory colorInventory;
    [SerializeField] List<GameObject> colorSlotContainers;
    [SerializeField] Image fadeToBlackImg;
    [SerializeField] Image sylviaLoading;

    [SerializeField] Sprite[] LoadingSprites;

    public bool loaded = false;
    private bool loadScreenFinished;

    [SerializeField] GameObject lightbox;

    //Containers for the various menus.
    [SerializeField] GameObject pauseMenuContainer;
    [SerializeField] GameObject mapContainer;
    [SerializeField] GameObject inventoryContainer;

    //Bools for tracking which menu is open.
    private bool anyMenuOpen = false;
    private bool pauseMenuOpen = false;
    private bool mapOpen = false;
    private bool inventoryOpen = false;

    public bool disablePausing = false;

    //When UIController is loaded, sends out action.
    public UnityAction UILoaded;
    public UnityAction ColorSlotAmountChanged; 

    //Input actions for opening the various menus.
    [SerializeField] InputActionReference openPauseMenu;
    [SerializeField] InputActionReference openMap;
    [SerializeField] InputActionReference openInventory;

    //Reference to player movement to freeze the player.
    private PlayerMovement playerMovement;

    private void OnEnable() {
        //StartCoroutine(FadeOutCoroutine(true));
        colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        colorInventory.onColorSlotsChanged += colorSlotUpdate;
        colorSlotUpdate();

        openPauseMenu.action.performed += OpenPauseMenu;
        openMap.action.performed += OpenMap;
        openInventory.action.performed += OpenInventory;
        lightbox.SetActive(false);
        pauseMenuContainer.SetActive(false);
        mapContainer.SetActive(false);
        inventoryContainer.SetActive(false);
    }

    private void OnDisable() {
        colorInventory.onColorSlotsChanged -= colorSlotUpdate;
        openPauseMenu.action.performed -= OpenPauseMenu;
        openMap.action.performed -= OpenMap;
        openInventory.action.performed -= OpenInventory;
    }


    private void colorSlotUpdate() {
        foreach(GameObject colorSlotContainer in colorSlotContainers) {
            colorSlotContainer.SetActive(false);
        }

        var initialSlotCount = 3;
        colorSlotContainers[colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
        ColorSlotAmountChanged?.Invoke();
    }

    private void OpenPauseMenu(InputAction.CallbackContext ctx) {OpenPauseMenu();}
    /// <summary>
    /// Opens the pause menu and closes all other menus.
    /// </summary>
    public void OpenPauseMenu() {
        pauseMenuOpen = !pauseMenuOpen;
        if(pauseMenuOpen) {
            Pause();
        } else {
            Resume();
        }
        anyMenuOpen = pauseMenuOpen;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        lightbox.SetActive(pauseMenuOpen);
        playerMovement.movementRoot.SetTotalRoot("menuOpen", pauseMenuOpen);
        mapOpen = false;
        mapContainer.SetActive(mapOpen);
        inventoryOpen = false;
        inventoryContainer.SetActive(inventoryOpen);
    }

    /// <summary>
    /// Opens the map menu and closes all other menus.
    /// </summary>
    private void OpenMap(InputAction.CallbackContext ctx) {OpenMap();}
    public void OpenMap() {
        mapOpen = !mapOpen;
        if(mapOpen) {
            Pause();
        } else {
            Resume();
        }
        anyMenuOpen = mapOpen;
        mapContainer.SetActive(mapOpen);
        lightbox.SetActive(mapOpen);
        playerMovement.movementRoot.SetTotalRoot("menuOpen", mapOpen);
        pauseMenuOpen = false;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        inventoryOpen = false;
        inventoryContainer.SetActive(inventoryOpen);
    }

    private void OpenInventory(InputAction.CallbackContext ctx) {OpenInventory();}
    /// <summary>
    /// Opens the inventory menu and closes all other menus.
    /// </summary>
    public void OpenInventory() {
        inventoryOpen = !inventoryOpen;
        if(inventoryOpen) {
            Pause();
        } else {
            Resume();
        }
        anyMenuOpen = inventoryOpen;
        inventoryContainer.SetActive(inventoryOpen);
        lightbox.SetActive(inventoryOpen);
        playerMovement.movementRoot.SetTotalRoot("menuOpen", inventoryOpen);
        pauseMenuOpen = false;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        mapOpen = false;
        mapContainer.SetActive(mapOpen);
    }

    /// <summary>
    /// Pauses the game by setting timescale to 0.
    /// </summary>
     private void Pause()
    {
        if(disablePausing) return;
        Time.timeScale = 0f;
        Debug.Log("Game is paused...");

    }

    /// <summary>
    /// Resumes the game by setting timescale to 1.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        Debug.Log("Game is resumed...");

    }

    private IEnumerator Loading()
    {
        int count = 0;
        sylviaLoading.gameObject.SetActive(true);
        while (!loaded)
        {
            sylviaLoading.sprite = LoadingSprites[count];
            count = (count + 1) % LoadingSprites.Length;
            yield return new WaitForSeconds(0.1f);
        }
        sylviaLoading.gameObject.SetActive(false);
        loadScreenFinished = true;
        UILoaded?.Invoke();
    }

    public IEnumerator FadeOutCoroutine(bool fadeIn, [CanBeNull] Func<IEnumerator> doLater = null)
    {
        int direction = fadeIn ? -1 : 1;
        fadeToBlackImg.color = new Color(0, 0, 0, fadeIn ? 1 : 0);

        if (fadeIn)
        {
            loadScreenFinished = false;
            StartCoroutine(Loading());
            while (!loadScreenFinished) 
                yield return new WaitForEndOfFrame();

            playerMovement.movementRoot.SetTotalRoot("loading", false);
            playerMovement.GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
        }

        while ((fadeToBlackImg.color.a < 1 && !fadeIn) || (fadeToBlackImg.color.a > 0 && fadeIn))
        {
            fadeToBlackImg.color = new Color(0, 0, 0, fadeToBlackImg.color.a + Time.deltaTime * direction);
            yield return new WaitForEndOfFrame();
        }
        
        if (!fadeIn)
        {
            loaded = false;
            sylviaLoading.gameObject.SetActive(true);
            StartCoroutine(Loading());
        }

        if (doLater != null)
            StartCoroutine(doLater());

    }
}
