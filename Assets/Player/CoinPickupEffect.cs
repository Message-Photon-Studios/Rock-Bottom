using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickupEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(waitAndKill());
    }

    private IEnumerator waitAndKill()
    {
        yield return new WaitForSeconds(11 / 24.0f);
        Destroy(gameObject);
    }
}
