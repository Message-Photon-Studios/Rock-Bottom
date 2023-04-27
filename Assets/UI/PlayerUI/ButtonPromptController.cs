using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonPromptController : MonoBehaviour
{
    //Map Prompts
    [SerializeField] InputActionReference rotateAction;
    [SerializeField] Image leftPromt;
    [SerializeField] Image rightPromt;
    [SerializeField] Sprite qSprite;
    [SerializeField] Sprite eSprite;
    [SerializeField] Sprite lbSprite;
    [SerializeField] Sprite rbSprite;

    //Map prompt
    [SerializeField] InputActionReference mapAction;
    [SerializeField] Image mapPromt;
    [SerializeField] Sprite mSprite;
    [SerializeField] Sprite ltSprite;

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
                mapPromt.sprite = mSprite;
                currentLayout = inputType;
            } else {
                leftPromt.sprite = lbSprite;
                rightPromt.sprite = rbSprite;
                mapPromt.sprite = ltSprite;
                currentLayout = inputType;
            }
        }
    }
}
