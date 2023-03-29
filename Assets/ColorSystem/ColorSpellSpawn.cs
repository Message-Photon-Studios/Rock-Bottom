using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns another color spell at impact
/// </summary>
public class ColorSpellSpawn : ColorSpell
{
    /// <summary>
    /// The spell that will be spawned on impact
    /// </summary>
    [SerializeField] GameObject spawnPrefab;
    
    protected override void Impact(Collision2D other)
    {
        GameObject obj = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<ColorSpell>().Initi(colorEffect, power*powerScale, player);
    }
}
