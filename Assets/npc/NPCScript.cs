using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCScript : MonoBehaviour
{
    [SerializeField] GameObject infoCanvas;
    [SerializeField] GameObject speechAlert;

    //List holding all the possible texts for the npc.
    [SerializeField] List<GameObject> texts;

    //Pointer towards current text
    private int currentText;

    [SerializeField] GameObject nextPrompt;

    [SerializeField] private InputActionReference interact;

    private bool isInside;

    private void OnEnable() {
        currentText = 0;
        isInside = false;
        if(texts.Count > 1) {
            interact.action.performed += NextText;
        } else {
            nextPrompt.SetActive(false);
        }
        LoadText();
    }

    private void OnDisable() {
        if (texts.Count > 1) {
            interact.action.performed -= NextText;
        }
    }

    private void LoadText() {
        foreach(GameObject text in texts) {
            text.SetActive(false);
        }
        texts[currentText].SetActive(true);
    }

    private void NextText(InputAction.CallbackContext ctx) {
        if (infoCanvas.activeSelf & isInside)
        {
            if(currentText + 1 == texts.Count) {
                EnableText(false);
            } else {
            currentText = (currentText + 1) % (texts.Count);
            LoadText();
            }
        }else if(!infoCanvas.activeSelf & isInside) {
            EnableText(true);
            currentText = 0;
            LoadText();
        }
    }

    private void EnableText(bool input) {
        infoCanvas.SetActive(input);
        speechAlert.SetActive(!input);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            EnableText(true);
            isInside = true;
            LoadText();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnableText(false);
            isInside = false;
            currentText = 0;
        }
    }
}
