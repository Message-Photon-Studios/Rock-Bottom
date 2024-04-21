using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadedAchievement : Achievement
{
    [SerializeField] string sceneName;

    protected override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += LevelLoaded;
    }
    void LevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name.Equals(sceneName)) RewardAchievement();
    }
}
