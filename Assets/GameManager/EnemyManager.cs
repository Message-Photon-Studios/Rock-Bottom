using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float enemyScaling = 0.1f;
    [SerializeField] GameColor[] spawnableColors; //The colors that the list can spawn with

    /// <summary>
    /// Returns a random color that the list can spawn with
    /// </summary>
    /// <returns></returns>
    public GameColor GetRandomEnemyColor()
    {
        return spawnableColors[UnityEngine.Random.Range(0,spawnableColors.Length)];
    }
    
    public void ScaleEnemyStats()
    {
        if(!GameManager.instance) return;
        EnemyStats[] enemyStats = FindObjectsOfType<EnemyStats>(); 
        int levelNum = GameManager.instance.levelNum;
        float scaling = enemyScaling * (levelNum-1)+1;

        foreach (EnemyStats enemy in enemyStats)
        {
            enemy.ScaleEnemy(scaling);
        }
    }
}
