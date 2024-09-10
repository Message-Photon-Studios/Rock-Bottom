using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrifiedPigmentPickup : MonoBehaviour
{
    [SerializeField] string id;
    [SerializeField] Transform image;
    [SerializeField] float spawnChance = .5f;
    private Coroutine hoverCoroutine;
    void OnValidate()
    {
        #if UNITY_EDITOR
        if(id == "" && transform.parent != null)
        {
            id=name+transform.parent.name+Random.Range(0,1000000000);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }

    private void OnEnable() {

        if(GameManager.instance.IsPetrifiedPigmentPickedUp(id) || Random.Range(0f,1f)<.5f)
        {
            Destroy(gameObject);
        }
        hoverCoroutine = StartCoroutine(hoverAnimation());
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.PickedUpPetrifiedPigment(id);
            StopCoroutine(hoverCoroutine);
            Destroy(gameObject);
        }
    }

    private IEnumerator hoverAnimation()
    {
        while (true)
        {
            image.position = new Vector3(
                image.position.x, 
                image.position.y + Mathf.Sin(Time.time * 2) * 0.003f, 
                image.position.z);
            yield return new WaitForFixedUpdate();
        }
    }
}
