using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[System.Serializable]
public class GameData
{
    [SerializeField] private string saveFileVersion;
    public string startScene;
    public int petrifiedPigment;
    public int inspirationPoints;
    public string[] unlockedColorSpells;
    public string[] petrifiedPigmentPickedUp;
    public PermanentUpgrades permanentUpgrades;
    public SerializedDictionary<string, Tips> tipsDictionary;
    public SerializedDictionary<string, NpcData> npcData;
    public SerializedDictionary<string, int> permanentShopBuys = new SerializedDictionary<string, int>(); 

    public string GetSaveFileVersion()
    {
        return saveFileVersion;
    }

    /// <summary>
    /// On new game this constructor will set default values.
    /// </summary>
    public GameData (string saveFileVersion)
    {
        this.saveFileVersion = saveFileVersion;
        startScene = "Tutorial";
        unlockedColorSpells = new string[0];
        petrifiedPigment = 0;
        inspirationPoints = 0;
        petrifiedPigmentPickedUp = new string[0];
        tipsDictionary = new SerializedDictionary<string, Tips>();
        npcData = null;
        permanentUpgrades = new PermanentUpgrades();
        permanentShopBuys = new SerializedDictionary<string, int>();
    }
}
