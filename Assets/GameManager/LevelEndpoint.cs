using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEndpoint : MonoBehaviour
{
    [SerializeField] InputActionReference interactAction;
    [SerializeField] GameObject canvas;
    bool enableExit = false;

    void OnEnable()
    {
        if(interactAction)
            interactAction.action.performed += ExitLevel;
    }

    void OnDisable()
    {
        if(interactAction)
            interactAction.action.performed -= ExitLevel;
    }

    void ExitLevel (InputAction.CallbackContext ctx)
    {
        if(!enableExit) return;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndLevel();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            canvas.SetActive(true);
            enableExit = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            canvas.SetActive(false);
            enableExit = false;
        }
    }
}
