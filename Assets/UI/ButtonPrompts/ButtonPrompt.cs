using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AYellowpaper.SerializedCollections;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] SerializedDictionary<InputSchemes, GameObject> prompts;
    SerializedDictionary<string, GameObject> promptDictionary = new SerializedDictionary<string, GameObject>();

    void Start()
    {
        foreach (KeyValuePair<InputSchemes, GameObject> item in prompts)
        {
            promptDictionary.Add(item.Key.ToString(), item.Value);
        }

        ButtonPromptManager.instance.onUpdatedInputScheme += UpdatePrompt;
        UpdatePrompt(ButtonPromptManager.instance.GetCurrentScheme());
    }

    void OnDestroy()
    {
        ButtonPromptManager.instance.onUpdatedInputScheme -= UpdatePrompt;
    }

    void UpdatePrompt(string scheme)
    {
        if (!promptDictionary.ContainsKey(scheme)) return;
        foreach (KeyValuePair<string, GameObject> item in promptDictionary)
        {
            item.Value.SetActive(false);
        }

        promptDictionary[scheme].SetActive(true);
    }

    [System.Serializable]
    enum InputSchemes
    {
        Keyboard,
        KeyboardMouse,
        XboxController,
        PlayStation
    }
}
