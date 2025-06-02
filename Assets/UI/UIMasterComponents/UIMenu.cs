using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [SerializeField] public GameObject mainComponent;
    public void CloseMenu()
    {
        if (mainComponent == null)
        {
            Debug.LogWarning("UIMenu missing main component: " + transform.parent.name);
            return;
        }
        mainComponent.SetActive(false);
        BeforeClosing();
        UIManager.instance.MenuClosed(this);
    }

    public void OpenMenu()
    {
        BeforeOpening();
        mainComponent.SetActive(true);
        UIManager.instance.MenuOpened(this);
    }

    protected virtual void BeforeClosing()
    {}

    protected virtual void BeforeOpening()
    {}
}
