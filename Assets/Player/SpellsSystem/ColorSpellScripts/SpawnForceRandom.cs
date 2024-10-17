using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a force to the color spell when spawned
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SpawnForceRandom : SpellMover
{
    public Vector2 spawnForceMin;
    public Vector2 spawnForceMax;
    public override void Init (int lookDir)
    {
        Vector2 spawnForce = new Vector2(Random.Range(spawnForceMin.x, spawnForceMax.x), Random.Range(spawnForceMin.y, spawnForceMax.y));
        GetComponent<Rigidbody2D>().AddForce(spawnForce*new Vector2(lookDir, 1));
    }
}
