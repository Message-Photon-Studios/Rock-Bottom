using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonPromptController : MonoBehaviour
{
    [SerializeField] InputActionReference rotateAction;
    [SerializeField] Image leftPromt;
    [SerializeField] Image rightPromt;
    [SerializeField] Sprite qSprite;
    [SerializeField] Sprite eSprite;
    [SerializeField] Sprite lbSprite;
    [SerializeField] Sprite rbSprite;
    private string currentLayout;
    
    private void OnEnable() {
        rotateAction.action.performed += CheckInputType;
    }

    private void OnDisable() {
        rotateAction.action.performed -= CheckInputType;
    }

    private void CheckInputType(InputAction.CallbackContext ctx) {
        string inputType = ctx.action.activeControl.device.name;
        if(currentLayout != inputType) {
            if(inputType == "Keyboard") {
                leftPromt.sprite = qSprite;
                rightPromt.sprite = eSprite;
                currentLayout = inputType;
            } else if(inputType == "Gamepad"){
                leftPromt.sprite = lbSprite;
                rightPromt.sprite = rbSprite;
                currentLayout = inputType;
            }
        }
    }
}
