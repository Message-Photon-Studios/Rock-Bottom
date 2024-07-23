using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : ItemLock
{
    protected override void OpenLock()
    {
        gameObject.SetActive(false); //TODO fix this later
    }
}
