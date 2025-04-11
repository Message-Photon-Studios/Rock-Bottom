using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Localization;

public class NpcManager : MonoBehaviour, IDataPersistence
{
    public static NpcManager instance = null;
    [SerializeField] public SerializedDictionary<string, NpcData> npcData = new SerializedDictionary<string, NpcData>();
    [SerializeField, HideInInspector] private SerializedDictionary<string, NpcData> npcOriginalData = new SerializedDictionary<string, NpcData>();
    
    public void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            Destroy(gameObject);
        }

        npcOriginalData = npcData;
    }

    void OnValidate()
    {
        #if UNITY_EDITOR
        
        foreach (KeyValuePair<string, NpcData> data in npcData)
        {
            data.Value.SetDialogueId();
        }

        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    public Dialogue GetDialogue(string npcName)
    {
        if(!npcData.ContainsKey(npcName))
        {
            if(npcOriginalData.ContainsKey(npcName))
            {
                npcData.Add(npcName, npcOriginalData[npcName]);
                Debug.LogWarning("No default dialogue for " + npcName + " in save file. Added npc from update");
            }
            else
            {
                Debug.LogError("No default dialogue for " + npcName);
                return null;
            }
        }

        Dialogue toReturn = npcData[npcName].GetDialogue(SceneManager.GetActiveScene().name);
        return toReturn;
    }

    #region SaveLoad
    void IDataPersistence.LoadData(GameData data)
    {
        if(data.npcData != null && data.npcData.Count > 0)
        {
            npcData = data.npcData;

            foreach (KeyValuePair<string, NpcData> npc in npcOriginalData)
            {
                if(!npcData.ContainsKey(npc.Key)) npcData.Add(npc.Key, npc.Value);
                else
                {
                    npcData[npc.Key].UpdateDialogues(npc.Value);
                }
            }
        }
        else
            npcData = npcOriginalData;
    }

    void IDataPersistence.SaveData(GameData data)
    {
        data.npcData = npcData;
    }
    #endregion
}

#region NpcData
[System.Serializable]
public class NpcData
{
    [SerializeField] Dialogue defaultDialogue;
    [SerializeField] SerializedDictionary<string, Dialogue> regionalDefaultDialogues;
    [SerializeField] SerializedDictionary<string, List<Dialogue>> specificDialogues;


    public void UpdateDialogues(NpcData original)
    {
        if(defaultDialogue.id == original.defaultDialogue.id) defaultDialogue = original.defaultDialogue;

        List<(string name, Dialogue dialogue)> toChange = new List<(string, Dialogue)>();

        foreach (KeyValuePair<string, Dialogue> regDef in regionalDefaultDialogues)
        {
            if(original.regionalDefaultDialogues.ContainsKey(regDef.Key) && regDef.Value.id == original.regionalDefaultDialogues[regDef.Key].id)
            {
                toChange.Add((regDef.Key, original.regionalDefaultDialogues[regDef.Key]));
            }
        }

        for (int i = 0; i < toChange.Count; i++)
        {
            regionalDefaultDialogues[toChange[i].name] = toChange[i].dialogue;
        }

        foreach (KeyValuePair<string, List<Dialogue>> spec in specificDialogues)
        {
            if(!original.specificDialogues.ContainsKey(spec.Key)) continue;
            
            for (int i = 0; i < spec.Value.Count; i++)
            {
                Dialogue getOriginal = original.specificDialogues[spec.Key].Find(find => (find.id == spec.Value[i].id));
                if(getOriginal == null) continue;
                specificDialogues[spec.Key][i] = getOriginal;
            }
        }
    }

    public void SetDialogueId()
    {
        defaultDialogue.UpdateId();
        foreach (KeyValuePair<string, Dialogue> regDef in regionalDefaultDialogues)
        {
            regDef.Value.UpdateId();    
        }

        foreach (KeyValuePair<string, List<Dialogue>> spec in specificDialogues)
        {
            foreach (Dialogue dia in spec.Value)
            {
                dia.UpdateId();
            }
        }
    }
    
    public Dialogue GetDialogue(string levelName)
    {
        if(specificDialogues.ContainsKey(levelName))
        {
            List<Dialogue> dialogues = specificDialogues[levelName];
            Dialogue toReturn = dialogues[0];
            specificDialogues[levelName].RemoveAt(0);
            if(specificDialogues[levelName].Count == 0) specificDialogues.Remove(levelName);
            return toReturn;
        }

        if(regionalDefaultDialogues.ContainsKey(levelName))
        {
            Debug.Log("regional");
            return regionalDefaultDialogues[levelName];
        }
        Debug.Log("default");
        return defaultDialogue;
    }

    public void AddSpecialDialogue (string level, Dialogue dialogue)
    {
        if(specificDialogues.ContainsKey(level))
            specificDialogues[level].Add(dialogue);
        else
            specificDialogues.Add(level, new List<Dialogue>{dialogue});
    }

    public void ChangeDefaultDialogue(Dialogue dialogue)
    {
        defaultDialogue = dialogue;
    }

    public void ChangeDefaultDialogue(string[] levels, Dialogue dialogue)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if(regionalDefaultDialogues.ContainsKey(levels[i]))
                regionalDefaultDialogues[levels[i]] = dialogue;
            else
                regionalDefaultDialogues.Add(levels[i], dialogue);
        }
    }

    public void ChangeDefaultDialogue(string level, Dialogue dialogue)
    {
        ChangeDefaultDialogue(new string[]{level}, dialogue);
    }
}

#endregion

#region Dialogue

[System.Serializable]
public class Dialogue
{
    public void UpdateId()
    {
        if(id == 0)
        {
            id = Random.Range(0,1000000000);
        }
    }
    [SerializeField] public int id = 0;
    [SerializeField] public LocalizedString[] texts;
} 

#endregion