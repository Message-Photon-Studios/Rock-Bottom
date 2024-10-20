using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a force to the color spell when spawned
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SpawnForce : SpellMover
{
    public Vector2 spawnForce;
    public override void Init (int lookDir)
    {
        GetComponent<Rigidbody2D>().AddForce(spawnForce*new Vector2(lookDir, 1));
    }
}
