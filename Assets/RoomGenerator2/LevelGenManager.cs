using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenManager : MonoBehaviour
{
    private LevelGeneration2 levelGen;

    public int size;

    public void init()
    {
        levelGen = new LevelGeneration2();
        levelGen.generate(size);
    }

    public void reset()
    {
        levelGen = new LevelGeneration2();
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
