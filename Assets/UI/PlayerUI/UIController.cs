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
    [SerializeField] public BigMenu pauseMenu;
    [SerializeField] public UIMenu settings;
    [SerializeField] public BigMenu map;
    [SerializeField] public BigMenu inventory;

    //When UIController is loaded, sends out action.
    public UnityAction UILoaded;
    public UnityAction ColorSlotAmountChanged; 

    [SerializeField] GameObject[] hideSlots;

    public UnityAction<Sprite, String> inspired;

    //Reference to player movement to freeze the player.
    void Start()
    {
        Player.instance.colorInventory.onColorSlotsChanged += colorSlotUpdate;
        colorSlotUpdate();
        
        Player.instance.mapAction += map.OpenMenu;
        Player.instance.inventoryAction += inventory.OpenMenu;

        lightbox.SetActive(false);
        map.CloseMenu();
        inventory.CloseMenu();
        pauseMenu.CloseMenu();
        loadingText.gameObject.SetActive(false);

        if(hideSlots.Length>0)
        foreach(GameObject slot in hideSlots) slot.SetActive(false);
    }

    void OnDestroy(){
        Player.instance.colorInventory.onColorSlotsChanged -= colorSlotUpdate;
        Player.instance.mapAction -= map.OpenMenu;
        Player.instance.inventoryAction -= inventory.OpenMenu;
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

        var initialSlotCount = 2;
        colorSlotContainersStuck[Player.instance.colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
        colorSlotContainersRotate[Player.instance.colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
        
        ColorSlotAmountChanged?.Invoke();
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
