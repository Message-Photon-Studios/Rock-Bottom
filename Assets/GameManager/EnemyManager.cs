using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameColor[] spawnableColors; //The colors that the enemies can spawn with

    /// <summary>
    /// Returns a random color that the enemies can spawn with
    /// </summary>
    /// <returns></returns>
    public GameColor GetRandomEnemyColor()
    {
        return spawnableColors[UnityEngine.Random.Range(0,spawnableColors.Length)];
    }
}
