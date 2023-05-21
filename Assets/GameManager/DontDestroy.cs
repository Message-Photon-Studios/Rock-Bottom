using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    public void disableChildren()
    {
        // Disable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void enableChildren()
    {
        // Enable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
