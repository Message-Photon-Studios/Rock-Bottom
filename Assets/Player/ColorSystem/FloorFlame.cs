using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFlame : MonoBehaviour
{
    [SerializeField] float maxForce;
    [SerializeField] float minForce;
    private List<GameObject> burnQueue = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 0.6f)).normalized * Random.Range(maxForce, minForce));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {     
        if (collision.gameObject.CompareTag("Enemy") && !burnQueue.Contains(collision.gameObject))
        {
            burnQueue.Add(collision.gameObject);
            Debug.Log("collide");
        }
    }

    public List<GameObject> ToBurn()
    {
        return burnQueue;
    }

    public void ClearBurnQueue()
    {
        burnQueue = new List<GameObject>();
    }
}
