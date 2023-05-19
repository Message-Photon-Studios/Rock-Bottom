using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRenderLayer : MonoBehaviour
{
    public void FixRenderLayers()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in spriteRenderers)
        {
            sprite.sortingOrder = (int)(sprite.transform.position.z * -20);
        }
    }
}
