using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns other color spells at impact
/// </summary>
public class ColorSpellSpawnImpact : SpellImpact
{
    /// <summary>
    /// After this time the spell will autospawn the spawn spells.
    /// </summary>
    [SerializeField] float detonateTimer;

    /// <summary>
    /// The spells that will be spawned on impact
    /// </summary>
    [SerializeField] GameObject[] spawnPrefabs;

    private void Update() {
        detonateTimer -= Time.deltaTime;
        if(detonateTimer <= 0)
        {
            Impact(null);
            Destroy(gameObject);
        }
    }
    
    public override void Impact(Collider2D other)
    {
        foreach (GameObject spawnPrefab in spawnPrefabs)
        {
            GameObject obj = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
            obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir);
        }
    }
}
