using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotifyInventory : MonoBehaviour
{
    public UnityAction onInventoryOpened;
   private void OnEnable() {
    onInventoryOpened?.Invoke();
   }
}
