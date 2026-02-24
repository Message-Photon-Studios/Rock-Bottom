using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiOveralyAssigner : MonoBehaviour
{
    void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();

        if(canvas.worldCamera == null)
        {
            canvas.worldCamera = GameObject.Find("UI Cam").GetComponent<Camera>();
            canvas.sortingLayerName = "UI";
        }
    }
}
