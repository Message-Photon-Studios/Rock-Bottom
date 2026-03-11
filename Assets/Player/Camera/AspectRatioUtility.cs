using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
    void Start()
    {
        Adjust();
    }

    public void Adjust()
    {
        float targetaspect = 16f / 9f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();

        if(scaleheight < 1.0f)
        {
            Rect rect = camera.rect;
        
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1f - scaleheight) / 2f;

            camera.rect = rect;
        } else
        {
            float scalewidth = 1f / scaleheight;

            Rect rect = camera.rect;
        
            rect.width = scalewidth;
            rect.height = 1f;
            rect.x = (1f - scalewidth) / 2f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
    
}
