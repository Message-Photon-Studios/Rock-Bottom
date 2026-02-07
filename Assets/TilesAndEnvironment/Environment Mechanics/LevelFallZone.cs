using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFallZone : MonoBehaviour
{
    [SerializeField] string goToLevel;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            LevelManager.instance.EndLevel(goToLevel);
        }
    }
}
