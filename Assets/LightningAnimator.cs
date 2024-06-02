using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAnimator : MonoBehaviour
{

    LineRenderer line;
    [SerializeField] private Texture[] textures;
    private int aniStep;
    [SerializeField] private float fps = 15;
    private float fpsCounter;
    private GameObject source;
    private GameObject target;
    private int hasSource = 0;
    private int hasTarget = 0;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            aniStep++;
            if (aniStep == textures.Length) aniStep = 0;
            line.material.SetTexture("_MainTex", textures[aniStep]);
            fpsCounter = 0f;
            if (source) line.SetPosition(0, source.transform.position);
            if (target) line.SetPosition(1, target.transform.position);
        }
    }

    public void SetSource(GameObject source)
    {        
        this.source = source;
    }

    public void SetTarget(GameObject target)
    {       
        this.target = target;
    }
}
