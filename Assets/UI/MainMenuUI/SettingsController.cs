using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsController : MonoBehaviour
{
    [SerializeField] MainMenuController controller;
    [SerializeField] InputActionReference exitSettingsMenu;
    private void OnEnable()
    {
        exitSettingsMenu.action.performed += ExitSettings;
    }

    void OnDisable()
    {
        exitSettingsMenu.action.performed -= ExitSettings;
    }

    void ExitSettings(InputAction.CallbackContext ctx)
    {
        controller.hideSettings();
    }

}
