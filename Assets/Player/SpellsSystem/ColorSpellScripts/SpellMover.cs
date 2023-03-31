using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a class that handles a spells movement
/// </summary>
public abstract class SpellMover : MonoBehaviour
{
    public abstract void Init(int lookDir);
}
