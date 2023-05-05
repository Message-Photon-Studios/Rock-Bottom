using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LightboxController : MonoBehaviour
{

    //action that is called when using the specified button.
    [SerializeField] InputActionReference inputAction;
    
    //is container open
    private bool open;

    //container holding all components.
    [SerializeField] GameObject container;

    //Lightbox for compoment
    [SerializeField] GameObject lightbox;

    //specified root.
    [SerializeField] string movementRoot;
    private PlayerMovement playerMovement;
    
    private void OnEnable() {
        inputAction.action.performed += OpenContainer;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        open = false;
    }

    private void OnDisable() {
        inputAction.action.performed -= OpenContainer;
    }

    /// <summary>
    /// on inputAction called, inverts state of open bool and sets container components
    /// according to the bool. 
    /// </summary>
    /// <param name="ctx"></param> Dunno what this does tbh.
    private void OpenContainer(InputAction.CallbackContext ctx) {
        open = !open;
        container.SetActive(open);
        lightbox.SetActive(open);
        playerMovement.movementRoot.SetTotalRoot(movementRoot, open);
    }
}
