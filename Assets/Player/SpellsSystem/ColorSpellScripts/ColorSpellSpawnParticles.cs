using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns particles with same color as spell
/// </summary>
public class ColorSpellSpawnParticles : MonoBehaviour
{
    /// <summary>
    /// After this time the particles  will autospawn the spawn spells.
    /// </summary>
    [SerializeField] bool spawnOneInstantly;
    [SerializeField] float spawnDelay;
    private float delayTimer;
    [SerializeField] int spawnAmount;
    [SerializeField] bool requirePlayerLOS;
    [SerializeField] ColorSpell spell;

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
        if (requirePlayerLOS)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Player.instance.transform.position - transform.position, Vector2.Distance(Player.instance.transform.position, transform.position), GameManager.instance.maskLibrary.onlySolidGround());
            if (hit) return;
        }
        foreach (ParticleSystem spawnPrefab in spawnPrefabs)
            {
                GameObject obj = GameObject.Instantiate(spawnPrefab.gameObject, transform.position, transform.rotation) as GameObject;
                var main = obj.GetComponent<ParticleSystem>().main;
                main.startColor = spell.GetColor().plainColor;
            }
    }
}
