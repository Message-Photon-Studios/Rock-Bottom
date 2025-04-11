using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowCoWorker : MonoBehaviour
{
    public void Work(IEnumerator _coroutine)
    {
        StartCoroutine(WorkCoroutine(_coroutine));
    }

    private IEnumerator WorkCoroutine(IEnumerator _coroutine)
    {
        yield return StartCoroutine(_coroutine);
        Destroy(this.gameObject);
    }
}
