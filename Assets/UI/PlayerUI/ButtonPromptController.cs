using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonPromptController : MonoBehaviour
{
    // List holding all actions related to button prompts.
    [SerializeField] List<InputActionReference> inputActions;

    //List holding the image and assosiated sprite for each prompt.
    [SerializeField] List<ButtonPromptSet> prompts;

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
                foreach(ButtonPromptSet prompt in prompts) {
                    prompt.image.sprite = prompt.keyboardImg;
                }
                currentLayout = inputType;
            } else {
                foreach(ButtonPromptSet prompt in prompts) {
                    prompt.image.sprite = prompt.controllerImg;
                }
                currentLayout = inputType;
            }
        }
    }

    [System.Serializable]
    public class ButtonPromptSet {
        public Image image;
        public Sprite keyboardImg;
        public Sprite controllerImg;
    }
}
