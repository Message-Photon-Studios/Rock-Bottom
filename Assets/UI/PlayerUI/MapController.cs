using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{

    //action that is called when using the map button.
    [SerializeField] InputActionReference mapAction;
    
    //is map open
    private bool open;

    //container holding all map components.
    [SerializeField] GameObject mapContainer;
    
    private void OnEnable() {
        mapAction.action.performed += OpenMap;
        open = false;
    }

    private void OnDisable() {
        mapAction.action.performed -= OpenMap;
    }

    /// <summary>
    /// on map action called, inverts state of open bool and sets map components
    /// according to the bool. 
    /// </summary>
    /// <param name="ctx"></param> Dunno what this does tbh.
    private void OpenMap(InputAction.CallbackContext ctx) {
        open = !open;
        mapContainer.SetActive(open);
    }
}
