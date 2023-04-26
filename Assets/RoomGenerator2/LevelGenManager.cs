using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static float cullDistance = 2.5f;
    private LevelGenerator levelGen;
    public GameObject player;

    public LevelArea levelType;
    public int size;

    private readonly string[] paths = {"Rooms/CrystalCaves/", "Rooms/PebbleArea"};

    public void init()
    {
        levelGen = new LevelGenerator();
        levelGen.generate(size, paths[(int)levelType]);
        GetComponent<ItemManager>().SpawnItems();
    }

    public void reset()
    {
        levelGen = new LevelGenerator();
        levelGen?.initGeneration(paths[(int)levelType]);
    }

    public void step()
    {
        levelGen?.stepGenerate(size, paths[(int)levelType]);
    }
#if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        levelGen?.graph.draw();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    private void Update()
    {
        levelGen?.cullElements();
    }

    private void FixedUpdate()
    {
        levelGen?.minimap?.testPosition(player.gameObject.transform.position);
    }
}
#endif