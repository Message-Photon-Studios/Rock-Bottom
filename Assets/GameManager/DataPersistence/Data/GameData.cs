using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string startScene;


    /// <summary>
    /// On new game this constructor will set default values.
    /// </summary>
    public GameData ()
    {
        startScene = "Tutorial";
    }
}
