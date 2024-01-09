using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Inspired : MonoBehaviour
{

    [SerializeField] Sprite spell;
    [SerializeField] String text;

    private bool triggered;

    private UIController UI;

    public void OnEnable() {
        triggered = false;
        UI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }
    
    public void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !triggered) {
            triggered = true;
            UI.inspired(spell, text);
        }
    }
}
