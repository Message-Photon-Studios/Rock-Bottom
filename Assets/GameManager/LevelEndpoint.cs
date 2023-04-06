using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndLevel();
        }
    }
}
