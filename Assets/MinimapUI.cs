using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{

    [SerializeField] RenderTexture map;
    [SerializeField] Image mapContainer;
    [SerializeField] Rect mapRect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Sprite sprite = Sprite.Create(toTexture2D(map), mapRect, Vector2.down);
        mapContainer.sprite = sprite;
    }

   Texture2D toTexture2D(RenderTexture rTex)
{
    Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
    // ReadPixels looks at the active RenderTexture.
    RenderTexture.active = rTex;
    tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
    tex.Apply();
    return tex;
}
}
