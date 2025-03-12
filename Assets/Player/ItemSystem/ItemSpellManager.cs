using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the distribution of items within the game
/// </summary>
public class ItemSpellManager : MonoBehaviour
{
    public static ItemSpellManager instance;
    [SerializeField] int itemPop;
    [SerializeField] public float stageCostMultiplier = 1;

    [SerializeField] int petrifiedPigmentDrops = 2;
    [SerializeField] ColorSpell[] levelSpells;

    private Dictionary<Type, List<ItemEffect>> itemEffectsInLevel = new Dictionary<Type, List<ItemEffect>>();

    private List<PetrifiedPigmentPickup> petrifiedPigments = new List<PetrifiedPigmentPickup>(0);

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Two ItemSpellManagers exist in this scene!!!");
        }
        instance = this;
        itemEffectsInLevel = new Dictionary<Type, List<ItemEffect>>();
    }

    /// <summary>
    /// Returns the item effects already spawned in level specified by effect
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public List<T> GetEffectsInLevel<T>(ItemEffect effectType) where T : ItemEffect
    {
        if(itemEffectsInLevel.ContainsKey(effectType.GetType()))
        {
            List<ItemEffect> effects = itemEffectsInLevel[effectType.GetType()];
            return effects.Cast<T>().ToList();
        } else
        return new List<T>();
    }

    public void AddSpawnedEffects(Item item)
    {
        List<ItemEffect> effects = item.effects;

        foreach (ItemEffect effect in effects)
        {
            if(itemEffectsInLevel.ContainsKey(effect.GetType()))
            {
                itemEffectsInLevel[effect.GetType()].Add(effect);
            } else
            {
                List<ItemEffect> newList = new List<ItemEffect>();
                newList.Add(effect);
                itemEffectsInLevel.Add(effect.GetType(), newList);
            }
        }
    }

    public void SpawnItems()
    {
        List<ColorSpell> spawnSetSpell = new List<ColorSpell>();
        List<ColorSpell> spawnableSpells = new List<ColorSpell>();

        foreach (ColorSpell spell in levelSpells)
        {
            if(GameManager.instance != null && GameManager.instance.IsSpellSpawnable(spell))
                spawnableSpells.Add(spell);
            else if(GameManager.instance == null)
            {
                spawnableSpells.Add(spell);
            }
        }

        spawnSetSpell.AddRange(spawnableSpells);
        
        
        List<GameObject> highSpawnChance = new List<GameObject>();
        List<GameObject> lowSpawnChance = new List<GameObject>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Item"))
        {
            ItemPickup pickup = obj.GetComponent<ItemPickup>();
            if(pickup == null) continue;
            if(obj == null) continue;
            if(pickup.spawnChance == SpawnPointChance.Guaranteed) continue;

            if(pickup.spawnChance == SpawnPointChance.HighChance) highSpawnChance.Add(obj);
            else lowSpawnChance.Add(obj);
        }


        //Purge items until they reached the allowed item pop count.
        while(highSpawnChance.Count + lowSpawnChance.Count > itemPop)
        {
            if(lowSpawnChance.Count > 0)
            {
                int i = UnityEngine.Random.Range(0, lowSpawnChance.Count);
                lowSpawnChance[i].SetActive(false);
                lowSpawnChance.RemoveAt(i);
            } else
            {
                int i = UnityEngine.Random.Range(0, highSpawnChance.Count);
                highSpawnChance[i].SetActive(false);
                highSpawnChance.RemoveAt(i);
            }
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

        for (int i = 0, j = 0; i < petrifiedPigmentDrops && j+i < petrifiedPigments.Count; i++)
        {
            int pick = i+j;
            if(GameManager.instance.IsPetrifiedPigmentPickedUp(petrifiedPigments[pick].GetID())) 
            {
                j++;
                i--;
                continue;
            }

            petrifiedPigments[pick].gameObject.SetActive(true);
        }
    }

    public void AddPetrifiedPigment(PetrifiedPigmentPickup petrifiedPigmentPickup)
    {
        petrifiedPigments.Add(petrifiedPigmentPickup);
    }

    public void ClearPetrifiedPigmentList()
    {
        petrifiedPigments = new List<PetrifiedPigmentPickup>();
    }
}
