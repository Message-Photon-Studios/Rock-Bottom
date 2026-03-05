using System.Collections;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public enum LevelArea
{
    CRYSTAL = 0,
    PEBBLE = 1
}

public class LevelGenManager : MonoBehaviour
{
    public static float ROOMSIZE = 4*.9f;
    public static int twoDoorRoomBias = 6;
    public static int threeDoorRoomBias = 2;

    public static float cullDistance = 3;
    private LevelGenerator levelGen;
    public GameObject player;
    public SpriteRenderer endCircle;

    public LevelArea levelType;
    public int level;
    public int size;

    public SerializedDictionary<DoorColor, int> regionSize;
    public int regionSizeMargin = 10;

    private readonly string[] paths = {"Rooms/CrystalCaves/Level_", "Rooms/PebbleArea/Level_"};
    private bool finished;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public IEnumerator generateSceneAsync(UIController canvas)
    {
        StartCoroutine(levelGen.insertPrefabsAsync(paths[(int)levelType]+level));
        while (!levelGen.instantiated)
            yield return new WaitForEndOfFrame();
        finishGen(canvas);
    }

    private void generateScene(UIController canvas)
    {
        levelGen.insertPrefabs(paths[(int)levelType]+level);
        finishGen(canvas);

    }

    #if(UNITY_EDITOR)
    void OnValidate()
    {
        size = 0;
        foreach (KeyValuePair<DoorColor, int> item in regionSize)
        {
            size += item.Value;
        }
    }
    #endif

    private void finishGen(UIController canvas)
    {
        GetComponent<ItemSpellManager>().SpawnItems();
        if (levelGen.endRoomPos != null)
            endCircle.transform.position = levelGen.endRoomPos.Value + Vector3.up * ROOMSIZE * 2;

        if (canvas != null)
            canvas.loaded = true;

        levelGen.ActivateHolders();
        finished = true;
        
        GetComponent<LevelManager>().FinishedGeneration();
    }
    
    /// <summary>
    /// If this var is set to positive then the generation will stop trying to generate after this ammount
    /// </summary>
    [SerializeField] int maxTries = -1;
    public IEnumerator init(UIController canvas, bool async)
    {
        levelGen = new LevelGenerator();
        levelGen.generationDone = false;
        StartCoroutine(levelGen.generateAll(size, paths[(int)levelType] + level, regionSize, regionSizeMargin, maxTries));

        while (!levelGen.generationDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (async)
            StartCoroutine(generateSceneAsync(canvas));
        else
            generateScene(canvas);

        yield return levelGen.tries;
    }

    public bool SceneGenerated()
    {
        return levelGen.generationDone;
    }
    
    public int LastGenerationTries()
    {
        return levelGen.tries;
    }

    public void reset()
    {
        levelGen = new LevelGenerator();
        levelGen?.initGeneration(paths[(int)levelType]+level);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        levelGen?.graph.draw();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
#endif

    private void Update()
    {
        if (finished)
            levelGen?.cullElements();
    }

    private void FixedUpdate()
    {
        levelGen?.minimap?.testPosition(player.gameObject.transform.position);
    }
}
