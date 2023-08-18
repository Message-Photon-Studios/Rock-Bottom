using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns other color spells at impact
/// </summary>
public class ColorSpellSpawn : SpellImpact
{
    /// <summary>
    /// After this time the spell will autospawn the spawn spells.
    /// </summary>
    [SerializeField] float spawnDelay;
    private float delayTimer;
    [SerializeField] int spawnAmount;

    /// <summary>
    /// The spells that will be spawned on impact
    /// </summary>
    [SerializeField] GameObject[] spawnPrefabs;

    private void Start()
    {
        delayTimer = spawnDelay;

    }


    private void Update() {
        if (spawnAmount<=0) return;
        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0)
        {
            //Impact(null);
            Spawn();
            spawnAmount--;
            delayTimer = spawnDelay;
        }
    }

    public void Spawn()
    {
        foreach (GameObject spawnPrefab in spawnPrefabs)
        {
            GameObject obj = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
            Debug.Log(spell.GetColor());
            obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir);
        }
    }
    
    public override void Impact(Collider2D other)
    {

    }
}
