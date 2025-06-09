using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrifiedPigmentPickup : InteractionObject
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
        if(ItemSpellManager.instance) ItemSpellManager.instance.AddPetrifiedPigment(this);
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        hoverCoroutine = StartCoroutine(hoverAnimation());
    }

    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);
        if(isClose)
        {
            GameManager.instance.PickedUpPetrifiedPigment(id);
            StopCoroutine(hoverCoroutine);
            Destroy(gameObject);
        }
    }

    protected override void PlayerInteract()
    {
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
