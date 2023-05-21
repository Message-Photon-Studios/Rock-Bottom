using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class TutorialEndpoint : MonoBehaviour
{
    [SerializeField] InputActionReference interactAction;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject trailer;
    [SerializeField] VideoPlayer player;

    [SerializeField] Sprite[] LoadingSprites;

    private UIController uiController;
    bool enableExit = false;

    void OnEnable()
    {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        if(interactAction)
            interactAction.action.performed += StartTrailer;
        player.loopPointReached += ExitLevel;
    }

    void OnDisable()
    {
        if(interactAction)
            interactAction.action.performed -= StartTrailer;
        player.loopPointReached -= ExitLevel;
    }

    void StartTrailer (InputAction.CallbackContext ctx)
    {
        if(!enableExit) return;
        DontDestroy bgMusic = GameObject.FindObjectOfType<DontDestroy>();
        bgMusic.disableChildren();
        trailer.SetActive(true);
        player.Play();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().movementRoot.SetTotalRoot("trailer", true);
    }

    void ExitLevel(VideoPlayer vp)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndLevel();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            canvas.gameObject.SetActive(true);
            enableExit = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            canvas.gameObject.SetActive(false);
            enableExit = false;
        }
    }
}
