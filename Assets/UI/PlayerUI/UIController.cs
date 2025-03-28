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
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] List<GameObject> colorSlotContainersRotate;
    
    [SerializeField] List<GameObject> colorSlotContainersStuck;
    [SerializeField] Image fadeToBlackImg;
    [SerializeField] Image sylviaLoading;

    [SerializeField] Sprite[] LoadingSprites;
    [SerializeField] TMP_Text loadingText;

    public GameObject tipsPanel;

    public bool loaded = false;
    private bool loadScreenFinished;

    [SerializeField] public GameObject lightbox;

    //Containers for the various menus.
    [SerializeField] GameObject pauseMenuContainer;
    [SerializeField] GameObject settingsContainer;
    [SerializeField] GameObject mapContainer;
    [SerializeField] GameObject inventoryContainer;

    //Bools for tracking which menu is open.
    private bool anyMenuOpen = false;
    private bool pauseMenuOpen = false;
    private bool settingsOpen = false;
    private bool mapOpen = false;
    private bool inventoryOpen = false;

    //When UIController is loaded, sends out action.
    public UnityAction UILoaded;
    public UnityAction ColorSlotAmountChanged; 

    //Input actions for opening the various menus.
    [SerializeField] InputActionReference openPauseMenu;
    [SerializeField] InputActionReference openMap;
    [SerializeField] InputActionReference openInventory;
    [SerializeField] InputActionReference closeTips;
    [SerializeField] GameObject[] hideSlots;

    public UnityAction<Sprite, String> inspired;

    //Reference to player movement to freeze the player.
    void Start()
    {
        Player.instance.colorInventory.onColorSlotsChanged += colorSlotUpdate;
        colorSlotUpdate();
        
        openPauseMenu.action.performed += OpenPauseMenu;
        openMap.action.performed += OpenMap;
        openInventory.action.performed += OpenInventory;
        closeTips.action.performed += CloseTips;

        lightbox.SetActive(false);
        pauseMenuContainer.SetActive(false);
        mapContainer.SetActive(false);
        inventoryContainer.SetActive(false);
        loadingText.gameObject.SetActive(false);

        if(hideSlots.Length>0)
        foreach(GameObject slot in hideSlots) slot.SetActive(false);
    }

    void OnDestroy(){
        Player.instance.colorInventory.onColorSlotsChanged -= colorSlotUpdate;
        openPauseMenu.action.performed -= OpenPauseMenu;
        openMap.action.performed -= OpenMap;
        openInventory.action.performed -= OpenInventory;
        closeTips.action.performed -= CloseTips;
    }

    public void UnlockColorSlots()
    {
        foreach(GameObject slot in hideSlots) slot.SetActive(true);
    }


    private void colorSlotUpdate() {
        foreach(GameObject colorSlotContainer in colorSlotContainersStuck) {
            colorSlotContainer.SetActive(false);
        }

        foreach (GameObject colorSlotContainer in colorSlotContainersRotate)
        {
            colorSlotContainer.SetActive(false);
        }

        var initialSlotCount = 3;
        colorSlotContainersStuck[Player.instance.colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
        colorSlotContainersRotate[Player.instance.colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
        
        ColorSlotAmountChanged?.Invoke();
    }

    private void OpenPauseMenu(InputAction.CallbackContext ctx) {OpenPauseMenu();}
    /// <summary>
    /// Opens the pause menu and closes all other menus.
    /// </summary>
    public void OpenPauseMenu() {
        GameManager.instance.tipsManager.CloseTips();
        pauseMenuOpen = !pauseMenuOpen;
        if(pauseMenuOpen) {
            GameManager.instance.Pause();
        } else {
            GameManager.instance.Resume();
        }
        anyMenuOpen = pauseMenuOpen;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        lightbox.SetActive(pauseMenuOpen);
        Player.instance.playerMovement.movementRoot.SetTotalRoot("menuOpen", pauseMenuOpen);
        mapOpen = false;
        mapContainer.SetActive(mapOpen);
        inventoryOpen = false;
        inventoryContainer.SetActive(inventoryOpen);
        settingsOpen = false;
        settingsContainer.SetActive(settingsOpen);
    }

    public void OpenSettings() {
        GameManager.instance.tipsManager.CloseTips();
        settingsOpen = !settingsOpen;
        if(settingsOpen) {
            GameManager.instance.Pause();
        } else {
            GameManager.instance.Resume();
        }
        anyMenuOpen = settingsOpen;
        settingsContainer.SetActive(settingsOpen);
        lightbox.SetActive(settingsOpen);
        Player.instance.playerMovement.movementRoot.SetTotalRoot("menuOpen", settingsOpen);
        pauseMenuOpen = false;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        mapOpen = false;
        mapContainer.SetActive(mapOpen);
        inventoryOpen = false;
        inventoryContainer.SetActive(inventoryOpen);
    }

    private void CloseTips(InputAction.CallbackContext ctx){CloseTips();}
    private void CloseTips()
    {
        GameManager.instance.tipsManager.CloseTips();
    }

    /// <summary>
    /// Opens the map menu and closes all other menus.
    /// </summary>
    private void OpenMap(InputAction.CallbackContext ctx) {OpenMap();}
    public void OpenMap() {
        GameManager.instance.tipsManager.CloseTips();
        mapOpen = !mapOpen;
        if(mapOpen) {
            GameManager.instance.Pause();
        } else {
            GameManager.instance.Resume();
        }
        anyMenuOpen = mapOpen;
        mapContainer.SetActive(mapOpen);
        lightbox.SetActive(mapOpen);
        Player.instance.playerMovement.movementRoot.SetTotalRoot("menuOpen", mapOpen);
        pauseMenuOpen = false;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        inventoryOpen = false;
        inventoryContainer.SetActive(inventoryOpen);
        settingsOpen = false;
        settingsContainer.SetActive(settingsOpen);
    }

    private void OpenInventory(InputAction.CallbackContext ctx) {OpenInventory();}
    /// <summary>
    /// Opens the inventory menu and closes all other menus.
    /// </summary>
    public void OpenInventory() {
        GameManager.instance.tipsManager.CloseTips();
        inventoryOpen = !inventoryOpen;
        if(inventoryOpen) {
            GameManager.instance.Pause();
        } else {
            GameManager.instance.Resume();
        }
        anyMenuOpen = inventoryOpen;
        inventoryContainer.SetActive(inventoryOpen);
        lightbox.SetActive(inventoryOpen);
        Player.instance.playerMovement.movementRoot.SetTotalRoot("menuOpen", inventoryOpen);
        pauseMenuOpen = false;
        pauseMenuContainer.SetActive(pauseMenuOpen);
        mapOpen = false;
        mapContainer.SetActive(mapOpen);
        settingsOpen = false;
        settingsContainer.SetActive(settingsOpen);
    }

    public void Inspired(Sprite spell, String text) {
        inspired?.Invoke(spell, text);
    }

    private IEnumerator Loading()
    {
        int count = 0;
        sylviaLoading.gameObject.SetActive(true);
        if(GameManager.instance != null)
            loadingText.text = GameManager.instance.tipsManager.GetLoadingTips();
        loadingText.gameObject.SetActive(true);
        while (!loaded)
        {
            sylviaLoading.sprite = LoadingSprites[count];
            count = (count + 1) % LoadingSprites.Length;
            yield return new WaitForSeconds(0.1f);
        }
        sylviaLoading.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
        loadScreenFinished = true;
        yield return new WaitForEndOfFrame();
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

            Player.instance.playerMovement.movementRoot.SetTotalRoot("loading", false);
            Player.instance.playerMovement.GetComponent<PlayerCombatSystem>().RemovePlayerAirlock();
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
