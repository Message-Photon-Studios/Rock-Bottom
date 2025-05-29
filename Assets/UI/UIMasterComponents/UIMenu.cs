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
        UIManager.instance.MenuClosed(this);
        BeforeClosing();
        mainComponent.SetActive(false);
    }

    public void OpenMenu()
    {
        UIManager.instance.MenuOpened(this);
        mainComponent.SetActive(true);
    }

    protected virtual void BeforeClosing()
    {}
}
