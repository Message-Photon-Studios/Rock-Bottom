using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpawnForce : MonoBehaviour
{
    public Vector2 spawnForce;
    void OnEnable()
    {
        GetComponent<Rigidbody2D>().AddForce(spawnForce);
    }
}
