using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Item[] spawnableItems;

    void OnEnable()
    {
        List<Item> spawnSet = new List<Item>();
        spawnSet.AddRange(spawnableItems);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Item"))
        {
            int rng = UnityEngine.Random.Range(0,spawnSet.Count);
            Item item = spawnSet[rng];
            spawnSet.RemoveAt(rng);
            obj.GetComponent<ItemPickup>().SetItem(item);
            if(spawnSet.Count <= 0) spawnSet.AddRange(spawnableItems);
        }
    }
}
