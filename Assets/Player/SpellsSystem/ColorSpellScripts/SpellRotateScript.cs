using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRotateScript : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 1;
    [SerializeField] bool InvertRotaion = false;
    [SerializeField] bool PlayerDirection = false;
    float RotationModifier = 1;

    private void Start()
    {
        if (PlayerDirection) RotationModifier = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().lookDir;
        if (InvertRotaion) RotationModifier *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,RotationSpeed * RotationModifier * Time.deltaTime);
    }
}
