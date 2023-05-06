using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotifyInventory : MonoBehaviour
{
    //When inventory is opened, 
    public UnityAction onInventoryOpened;

    /// <summary>
    /// On enable, notify that inventory is opened.
    /// </summary>
   private void OnEnable() {
    onInventoryOpened?.Invoke();
   }
}
