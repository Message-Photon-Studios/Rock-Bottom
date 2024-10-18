using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellOrderScript : MonoBehaviour
{
    public static int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = SpellOrderScript.counter++;
    }
}
