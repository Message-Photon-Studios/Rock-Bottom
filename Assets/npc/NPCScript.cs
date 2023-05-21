using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    [SerializeField] GameObject infoCanvas;
    [SerializeField] GameObject speechAlert;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            infoCanvas.SetActive(true);
            speechAlert.SetActive(false);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoCanvas.SetActive(false);
            speechAlert.SetActive(true);
        }
    }
}
