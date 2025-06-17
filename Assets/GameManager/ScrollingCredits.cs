using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class ScrollingCredits : BigMenu
{

    private RectTransform rect;

    [SerializeField] MainMenuController controller;

    void OnEnable()
    {
        // Set the rectTransform height to -90
        rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -700);
    }

    void FixedUpdate()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 1);

        if (rect.anchoredPosition.y > 2565.74)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -1100);
        }
    }

    protected override void AfterClosing()
    {
        controller.hideCredits();
    }

    protected override void AfterOpening()
    {
    }
}
