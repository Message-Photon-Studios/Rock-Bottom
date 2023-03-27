using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorEffect", menuName = "Gameplay Color/Color Effect/ColorEffect")]
public abstract class ColorEffect : ScriptableObject
{
    public abstract void Apply(GameObject enemy, GameObject player);
}
