using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string startScene;
    public int petrifiedPigment;
    public string[] unlockedColorSpells;

    /// <summary>
    /// On new game this constructor will set default values.
    /// </summary>
    public GameData ()
    {
        startScene = "Tutorial";
        unlockedColorSpells = new string[0];
        petrifiedPigment = 0;
    }
}
