using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnData
{
    public EnemyStats enemy;
    [Range(0,1)]
    public float spawnChance;
}

[System.Serializable]
public class EnemyList
{
    public List<EnemySpawnData> list;
}

[ExecuteInEditMode]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public EnemyList enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
