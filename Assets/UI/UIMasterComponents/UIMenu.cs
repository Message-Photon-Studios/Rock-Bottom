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
        BeforeClosing();
        mainComponent.SetActive(false);
        UIManager.instance.MenuClosed(this);
        AfterClosing();
    }

    public void OpenMenu()
    {
        BeforeOpening();
        mainComponent.SetActive(true);
        UIManager.instance.MenuOpened(this);
        AfterOpening();
    }

    protected virtual void BeforeClosing(){}
    protected virtual void BeforeOpening(){}
    protected virtual void AfterClosing(){}
    protected virtual void AfterOpening(){}
}
