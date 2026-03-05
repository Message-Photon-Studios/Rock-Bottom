using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorerAchievement : Achievement
{
    [SerializeField] string[] levelsToInclude;

    protected override void Start()
    {
        base.Start();
        GameManager.instance.onNewAreaFound += NewArea;
    }

    void OnDisable()
    {
        GameManager.instance.onNewAreaFound += NewArea;
    }

    void NewArea(string area)
    {
        if(levelsToInclude.Contains(area)) ProgressAchievement();
    }
}
