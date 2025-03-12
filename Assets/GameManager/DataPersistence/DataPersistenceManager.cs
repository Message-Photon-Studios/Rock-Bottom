using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string currentSaveFileVersion;
    [SerializeField] private string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance {get; private set;}
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Found several DataPersistenceManager");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

    }

    bool initiated = false;
    public void Start() 
    {
        if(initiated) return;
        initiated = true;
        
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        
        LoadGame();
    }   

    public bool SaveFileVersionOk()
    {
        if(gameData.GetSaveFileVersion() != null)
            return gameData.GetSaveFileVersion().Equals(currentSaveFileVersion);
        else return false;
    }

    public void NewGame()
    {
        this.gameData = new GameData(currentSaveFileVersion);

        dataHandler.Save(gameData);
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if(this.gameData == null)
        {
            Debug.Log("No data was found. Initializing new data.");
            NewGame();
        } else if(!SaveFileVersionOk())
        {
            Debug.Log("Save file version not compatible with new game. Initializing new data.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    #if UNITY_EDITOR
    
    [SerializeField] bool resetSave;

    void OnValidate()
    {
        if(resetSave)
        {
            ResetSaveEditor();
            resetSave = false;
        }
    }
    public void ResetSaveEditor()
    {
        NewGame();
        LoadGame();
        SaveGame();
    }
    #endif
}
