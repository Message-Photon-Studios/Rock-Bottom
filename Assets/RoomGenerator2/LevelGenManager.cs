using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenManager : MonoBehaviour
{
    private LevelGenerator levelGen;

    public int size;

    public void init()
    {
        levelGen = new LevelGenerator();
        levelGen.generate(size);
    }

    public void reset()
    {
        levelGen = new LevelGenerator();
        levelGen?.initGeneration();
    }

    public void step()
    {
        levelGen?.stepGenerate(size);
    }

    void OnDrawGizmos() 
    {
        levelGen?.graph.draw();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
}