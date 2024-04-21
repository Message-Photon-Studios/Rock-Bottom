using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyKilledSceneAchievement : Achievement
{
    [SerializeField] GameObject enemy;
    [SerializeField] string sceneName;

    protected override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += LoadedScene;
    }

    EnemyStats stats;
    void LoadedScene(Scene scene, LoadSceneMode mode)
    {
        if(!scene.name.Equals(sceneName))
        {
            if(stats != null)
            {
                stats.onEnemyDeath -= EnemyKilled;
                stats = null;
            }
            return;
        }

        GameObject obj = GameObject.Find(enemy.name);
        if(obj != null)
        { 
            stats = obj.GetComponent<EnemyStats>();
            obj.GetComponent<EnemyStats>().onEnemyDeath += EnemyKilled;
        }
    }


    void EnemyKilled()
    {
        RewardAchievement();
    }
}
