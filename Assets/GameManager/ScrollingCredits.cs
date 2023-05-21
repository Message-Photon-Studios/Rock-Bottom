using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class ScrollingCredits : MonoBehaviour
{

    private RectTransform rect; 

    [SerializeField] MainMenuController controller;
    [SerializeField] GameObject credits;
    [SerializeField] InputActionReference exitCreditsMenu;
    

    void OnEnable()
    {
        // Set the rectTransform height to -90
        rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -500);
        exitCreditsMenu.action.performed += ExitCreditsMenu;
    }

    void OnDisable()
    {
        exitCreditsMenu.action.performed -= ExitCreditsMenu;
    }

    void FixedUpdate()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 1);

        if (rect.anchoredPosition.y > 2619)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -850);
        }
    }

    void ExitCreditsMenu(InputAction.CallbackContext ctx)
    {
        controller.hideCredits();
    }
}
