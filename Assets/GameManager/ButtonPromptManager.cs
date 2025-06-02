using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ButtonPromptManager : MonoBehaviour
{
    #region Singleton

    public static ButtonPromptManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else enabled = false;
    }

    #endregion

    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference[] inputActions;

    public UnityAction<string> onUpdatedInputScheme;

    private string currentMap = "Keyboard";

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void OnEnable()
    {
        foreach (InputActionReference inputAction in inputActions)
        {
            inputAction.action.performed += CheckInputType;
        }
    }

    void OnDisable()
    {
        foreach (InputActionReference inputAction in inputActions)
        {
            inputAction.action.performed -= CheckInputType;
        }
    }

    public string GetCurrentScheme()
    {
        return currentMap;
    }

    private void CheckInputType(InputAction.CallbackContext ctx)
    {
        if(playerInput == null) playerInput = FindObjectOfType<PlayerInput>();
        string inputType = playerInput.currentControlScheme;

        if (!currentMap.Equals(inputType))
        {
            currentMap = inputType;
            onUpdatedInputScheme?.Invoke(currentMap);
        }
    }
}
