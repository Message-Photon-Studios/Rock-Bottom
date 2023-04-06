using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelGenManager levelGenerator;
    [SerializeField] string nextLevelName;
    private void Start() 
    {
        levelGenerator.init();
    }

    public void EndLevel()
    {
        SceneManager.LoadScene(0);
    }
}
