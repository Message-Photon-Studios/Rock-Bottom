using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMaster : MonoBehaviour
{
    public static UIMaster instance;
    public UIMenu currentlyOpen = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MenuOpened(UIMenu menu)
    {
        Debug.Log("Menu opened");
        if (currentlyOpen == menu)
        {
            menu.CloseMenu();
            currentlyOpen = null;
        }
        else if (currentlyOpen != null)
        {
            currentlyOpen.CloseMenu();
            currentlyOpen = menu;
        }
        else
        {
            currentlyOpen = menu;
        }
    }

    public void MenuClosed(UIMenu menu)
    {
        Debug.Log("Menu opened");
        if (currentlyOpen == menu)
        {
            currentlyOpen = null;
        }
    }
}
