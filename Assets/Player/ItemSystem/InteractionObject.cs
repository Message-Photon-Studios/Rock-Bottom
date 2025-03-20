using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractionObject : MonoBehaviour
{
    protected bool playerInRange {get; private set;} = false;

    protected virtual void Start()
    {
        Player.instance.interactAction += HandleInteractInput;
    }

    protected virtual void OnDestroy()
    {
        Player.instance.interactAction -= HandleInteractInput;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            PlayerClose(playerInRange);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            PlayerClose(playerInRange);
        }
    }

    void HandleInteractInput()
    {   
        if(playerInRange) PlayerInteract();
    }

    protected abstract void PlayerInteract();

    protected virtual void PlayerClose(bool isClose) {}
}
