using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbsUIMenu : MonoBehaviour
{
    [SerializeField] public GameObject mainObjMenu;
    public void CloseMenu()
    {
        UIMaster.instance.MenuClosed(this);
        mainObjMenu.SetActive(false);
    }

    public void OpenMenu()
    {
        UIMaster.instance.MenuOpened(this);
        mainObjMenu.SetActive(true);
    }
}
