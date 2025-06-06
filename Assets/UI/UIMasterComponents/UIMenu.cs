using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Base class for all menus in the game.
/// </summary>
public class UIMenu : MonoBehaviour
{
    /// <summary>
    /// Main component to be toggled on/off.
    /// </summary>
    [SerializeField] public GameObject mainComponent;

    /// <summary>
    /// Closes the menu. 
    /// When a menu is closed, allow for things to happen before and after closing.
    /// </summary>
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

    /// <summary>
    /// Opens the menu. 
    /// When a menu is opened, allow for things to happen before and after opening.
    /// </summary>
    public void OpenMenu()
    {
        if (mainComponent == null)
        {
            Debug.LogWarning("UIMenu missing main component: " + transform.parent.name);
            return;
        }
        BeforeOpening();
        mainComponent.SetActive(true);
        UIManager.instance.MenuOpened(this);
        AfterOpening();
    }

    /// <summary>
    /// A virtual method to allow classes to be seen as BigMenus for logics.
    /// Returns false if no override is done on child classes.
    /// </summary>
    /// <returns>If the UIMenu is a big menu or not.</returns>
    public virtual bool IsBigMenu()
    {
        return false;
    }
    
    protected virtual void BeforeClosing() { }
    protected virtual void BeforeOpening() { }
    protected virtual void AfterClosing() { }
    protected virtual void AfterOpening() { }
}
