using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonPromptControllerTutorial : MonoBehaviour
{
    // List holding all actions related to button prompts.
    [SerializeField] List<InputActionReference> inputActions;

    [SerializeField] GameObject keyboard;
    [SerializeField] GameObject controller;

    private string currentLayout;
    
    private void OnEnable() {
        foreach(InputActionReference action in inputActions) {
            action.action.performed += CheckInputType;
        }

    }

    private void OnDisable() {
        foreach(InputActionReference action in inputActions) {
            action.action.performed -= CheckInputType;
        }
    }

    private void CheckInputType(InputAction.CallbackContext ctx) {
        string inputType = ctx.action.activeControl.device.name;
        if(currentLayout != inputType) {
            if(inputType == "Keyboard") {
                keyboard.SetActive(true);
                controller.SetActive(false);
                currentLayout = inputType;
            } else {
                controller.SetActive(true);
                keyboard.SetActive(false);
                currentLayout = inputType;
            }
        }
    }
}
