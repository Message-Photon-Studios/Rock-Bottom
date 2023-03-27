using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private LevelGenerator levelGen;
    public int minRoomCount;
    public int maxDepth;
    [Range(1, 4)]
    public int maxBranchesPerRoom;
    [Range(0.02f, 1f)]
    public float probability;
    [Range(0f, 1f)]
    public float probabilityFalloff;
    [Range(0f, 1f)] 
    public float branchingBias;

    [Space(10)]

    [Range(0f, 1f)]
    public float bigRoomProbability;
    [Range(1f, 100f)]
    public int bigRoomMinDistance;

    public void init()
    {
        levelGen = new LevelGenerator();
        levelGen.generateMainStructure(
            probability, 
            probabilityFalloff, 
            maxDepth, 
            minRoomCount, 
            maxBranchesPerRoom, 
            branchingBias,
            bigRoomProbability,
            bigRoomMinDistance);
    }

    public void reset()
    {
        levelGen = null;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnDrawGizmos() 
    {
        levelGen?.graph.draw();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
