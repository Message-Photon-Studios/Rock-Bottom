using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Localization;

public class NPCScript : UIMenu
{
    [SerializeField] GameObject speechAlert;
    [SerializeField] GameObject mapIcon;

    //List holding all the possible texts for the npc.
    [SerializeField] TMP_Text textUi;
    [SerializeField] TMP_Text nameUi;

    //Pointer towards current text
    private int currentText;

    [SerializeField] GameObject nextPrompt;

    [SerializeField] private InputActionReference interact;

    private bool isInside;

    Dialogue dialogue;

    [Header("Complete Quest on Interact")]
    [SerializeField] string questNpc;
    [SerializeField] string questLocation, questName;

    void Start()
    {
        nameUi.text = name;
        if(mapIcon) mapIcon.SetActive(true);
    }
    private void OnEnable() {
        currentText = 0;
        interact.action.performed += NextText;
    }

    private void OnDisable() {
        interact.action.performed -= NextText;
    }

    private void NextText(InputAction.CallbackContext ctx) {

        if(!questNpc.Equals("")) NpcManager.instance.QuestUnlocked(questNpc, questLocation, questName);

        if(isInside)
        {
            if(dialogue == null)
            {
                dialogue = NpcManager.instance.GetDialogue(name);
            }

            if (mainComponent.activeSelf)
            {   
                currentText ++;

                if(currentText == dialogue.texts.Length) {
                    dialogue = NpcManager.instance.GetDialogue(name);
                    currentText = 0;
                    EnableText(false);
                    DataPersistenceManager.instance.SaveGame();
                }

                textUi.text = dialogue.texts[currentText].GetLocalizedString();

            } else 
            {
                EnableText(true);
                currentText = 0;
            }
        }
    }

    private void EnableText(bool input)
    {
        if (input == true)
        {
            OpenMenu();
            speechAlert.SetActive(false);
            if (dialogue == null) dialogue = NpcManager.instance.GetDialogue(name);
            textUi.text = dialogue.texts[currentText].GetLocalizedString();
        }
        else
        {
            CloseMenu();
            speechAlert.SetActive(true);
        }
    }

    protected override void BeforeClosing()
    {
        speechAlert.SetActive(true);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            EnableText(true);
            isInside = true;
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
