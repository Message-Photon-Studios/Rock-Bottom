using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{

    [SerializeField] float downDeg;
    [SerializeField] float force;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.rotation = Quaternion.EulerAngles(0, 0, downDeg * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody2D>()?.AddForce(force * new Vector2(Mathf.Cos((downDeg-90) * Mathf.Deg2Rad), Mathf.Sin((downDeg - 90) * Mathf.Deg2Rad)));
    }
}
