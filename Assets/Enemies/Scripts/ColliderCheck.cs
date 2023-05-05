using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class ColliderCheck : MonoBehaviour
{
    public Action<Collision2D> onCollision;
    void OnCollisionEnter2D(Collision2D other)
    {
        onCollision?.Invoke(other);
    }
}
