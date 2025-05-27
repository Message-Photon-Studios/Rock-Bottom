using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMaster : MonoBehaviour
{
    public static UIMaster instance;
    public AbsUIMenu currentlyOpen = null;
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

    public void MenuOpened(AbsUIMenu menu)
    {
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

    public void MenuClosed(AbsUIMenu menu)
    {
        if (currentlyOpen == menu)
        {
            currentlyOpen = null;
        }
    }
}
