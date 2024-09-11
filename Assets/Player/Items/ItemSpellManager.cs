using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the distribution of items within the game
/// </summary>
public class ItemSpellManager : MonoBehaviour
{
    [SerializeField] float stageCostMultiplier = 1;
    [SerializeField] Item[] spawnableItems;
    [SerializeField] ColorSpell[] levelSpells;

    public void SpawnItems()
    {
        List<Item> spawnSet = new List<Item>();
        List<ColorSpell> spawnSetSpell = new List<ColorSpell>();
        spawnSet.AddRange(spawnableItems);

        List<ColorSpell> spawnableSpells = new List<ColorSpell>();

        foreach (ColorSpell spell in levelSpells)
        {
            if(GameManager.instance.IsSpellSpawnable(spell))
                spawnableSpells.Add(spell);
        }

        spawnSetSpell.AddRange(spawnableSpells);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Item"))
        {
            ItemPickup pickup = obj.GetComponent<ItemPickup>();
            if(pickup == null) continue;
            pickup.RandomSpawnDestroy();
            if(obj == null) continue;
            if(pickup.setByhand) continue;
            Item item = null;
            int rng = 0;
            while(!item)
            {
                rng = UnityEngine.Random.Range(0,spawnSet.Count);
                item = spawnSet[rng];
                if(!item.CanBeSpawned())
                {
                    spawnSet.RemoveAt(rng);
                    item = null;
                }

                if(spawnSet.Count <= 0)
                {
                    spawnSet.AddRange(spawnableItems);
                }
            }


            spawnSet.RemoveAt(rng);
            pickup.SetItem(item, Mathf.RoundToInt(item.itemCost * stageCostMultiplier));
            if(spawnSet.Count <= 0) spawnSet.AddRange(spawnableItems);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpellItem"))
        {
            obj.GetComponent<SpellPickup>().RandomSpawnDestroy();
            if(obj == null) continue;
            int rng = UnityEngine.Random.Range(0,spawnSetSpell.Count);
            ColorSpell spell = spawnSetSpell[rng];
            spawnSetSpell.RemoveAt(rng);
            obj.GetComponent<SpellPickup>().SetSpell(spell);
            if(spawnSetSpell.Count <= 0) spawnSetSpell.AddRange(spawnableSpells);
        }
    }
}
