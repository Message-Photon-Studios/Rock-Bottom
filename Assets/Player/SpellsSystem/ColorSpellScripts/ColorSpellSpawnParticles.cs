using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns particles with same color as spell
/// </summary>
public class ColorSpellSpawnParticles : SpellImpact
{
    /// <summary>
    /// After this time the particles  will autospawn the spawn spells.
    /// </summary>
    [SerializeField] bool spawnOneInstantly;
    [SerializeField] float spawnDelay;
    private float delayTimer;
    [SerializeField] int spawnAmount;

    [SerializeField] ParticleSystem[] spawnPrefabs;

    private void Start()
    {
        delayTimer = spawnDelay;
        if (spawnOneInstantly) Spawn();
    }


    private void Update()
    {
        if (spawnAmount <= 0) return;
        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0)
        {
            Spawn();
            spawnAmount--;
            delayTimer = spawnDelay;
        }
    }

    public void Spawn()
    {
        foreach (ParticleSystem spawnPrefab in spawnPrefabs)
        {
            GameObject obj = GameObject.Instantiate(spawnPrefab.gameObject, transform.position, transform.rotation) as GameObject;
            var main = obj.GetComponent<ParticleSystem>().main;
            main.startColor = spell.GetColor().plainColor;
        }
    }

    public override void Impact(Collider2D other, Vector2 impactPoint)
    {

    }
}
