using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrifiedPigmentPickup : MonoBehaviour
{
    [SerializeField] string id;
    [SerializeField] Transform image;
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

    void Awake()
    {
        ItemSpellManager.instance.AddPetrifiedPigment(this);
        gameObject.SetActive(false);
    }

    private void Start()
    {
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

    public string GetID() { return id; }
}
