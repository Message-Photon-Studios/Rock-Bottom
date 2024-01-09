using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InspiredPopup : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] Image image;
    [SerializeField] TMP_Text inspiredText;
    [SerializeField] TMP_Text unlockedText;

    [SerializeField] GameObject lightBox;

    [SerializeField] UIController UIController;
    
    private void OnEnable() {
        container.SetActive(false);
        UIController.inspired += Load;
    }

    /// <summary>
    /// Sets up the correct img and text for the inspired screen and starts the process.
    /// </summary>
    /// <param name="spell">Image to be displayed</param>
    /// <param name="info">Accompanying text</param>
    public void Load(Sprite spell, string info) {
        
        image.sprite = spell;
        unlockedText.SetText(info);
        StartCoroutine(FadeCorutine(true));
        StartCoroutine(wait());
    
    }

    /// <summary>
    /// Wait 3 seconds, then start to fade out.
    /// </summary>
    /// <returns></returns>
    public IEnumerator wait() {
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeCorutine(false));
    }

    /// <summary>
    /// Corutine to fade in or out the inspiration screen depending on input.
    /// </summary>
    /// <param name="fadeIn">bool to dictate fadeIn or not</param>
    /// <returns></returns>
    public IEnumerator FadeCorutine(bool fadeIn) {
        int direction = -1;
        if(fadeIn){direction = 1; container.SetActive(true);}
        
        if(fadeIn) {lightBox.SetActive(true);}
        Color startColor = new Color(255, 255, 255, fadeIn ? 0 : 1);

        while ((startColor.a > 0 && !fadeIn) || (startColor.a < 1 && fadeIn))
        {
            startColor = new Color(255, 255, 255, startColor.a + Time.deltaTime * direction);
            image.color = startColor;
            inspiredText.color = startColor;
            unlockedText.color = startColor;
            yield return new WaitForEndOfFrame();
        }
        if(!fadeIn) {lightBox.SetActive(false); container.SetActive(false);}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
