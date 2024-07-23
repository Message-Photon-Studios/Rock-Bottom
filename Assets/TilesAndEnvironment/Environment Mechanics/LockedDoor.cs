using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : ItemLock
{
    [SerializeField] Animator animator;
    protected override void OpenLock()
    {
        animator.SetTrigger("OpenDoor");
    }
}
