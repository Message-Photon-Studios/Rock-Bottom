using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMusic : MonoBehaviour
{
    private static TutorialMusic instance;

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
            Destroy(gameObject);
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
