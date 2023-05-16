using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private ColorInventory colorInventory;
    [SerializeField] List<GameObject> colorSlotContainers;
    [SerializeField] Image fadeToBlackImg;
    [SerializeField] Image sylviaLoading;

    [SerializeField] Sprite[] LoadingSprites;

    public bool loaded = false;
    private bool loadScreenFinished;

    private void OnEnable() {
        StartCoroutine(FadeOutCoroutine(true));
        colorInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<ColorInventory>();
        colorInventory.onColorSlotsChanged += colorSlotUpdate;
        colorSlotUpdate();
    }

    private void OnDisable() {
        colorInventory.onColorSlotsChanged -= colorSlotUpdate;
    }


    private void colorSlotUpdate() {
        foreach(GameObject colorSlotContainer in colorSlotContainers) {
            colorSlotContainer.SetActive(false);
        }

        var initialSlotCount = 3;
        colorSlotContainers[colorInventory.colorSlots.Count - initialSlotCount].SetActive(true);
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
