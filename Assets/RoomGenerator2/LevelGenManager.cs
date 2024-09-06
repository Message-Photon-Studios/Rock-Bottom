using System.Collections;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

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

    public static float cullDistance = 3.5f;
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
        finished = true;
        
        GetComponent<LevelManager>().FinishedGeneration();
    }

    public void init(UIController canvas, bool async)
    {
        levelGen = new LevelGenerator();
        levelGen.generate(size, paths[(int)levelType]+level, regionSize, regionSizeMargin);
        if (async)
            StartCoroutine(generateSceneAsync(canvas));
        else
            generateScene(canvas);
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
