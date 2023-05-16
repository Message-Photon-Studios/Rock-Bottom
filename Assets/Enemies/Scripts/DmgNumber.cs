using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DmgNumber : MonoBehaviour
{
    [SerializeField] private Color nullColor;
    [SerializeField] private Color smallColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private int smallThreshold;
    [SerializeField] private int critThreshold;
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private float upwardsSpeed;
    [SerializeField] private float duration;
    [SerializeField] private string immuneMsg;
    private TextMeshPro textMesh;

    private float timer = 0;
    private float size = 1;
    
    void Update()
    {
        transform.position += Vector3.up * upwardsSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localScale = Vector3.one * size * sizeCurve.Evaluate(timer / duration);
        }
    }

    public static void create(float number, Vector2 position)
    {
        int numberInt = (int)number;
        var prefab = Resources.Load<DmgNumber>("DmgNumbers/DmgNumber");
        DmgNumber dmgNumber = Instantiate(prefab, position, Quaternion.identity);
        dmgNumber.textMesh = dmgNumber.GetComponent<TextMeshPro>();
        if (numberInt == 0)
            dmgNumber.textMesh.SetText(prefab.immuneMsg);
        else
            dmgNumber.textMesh.SetText(numberInt.ToString(CultureInfo.InvariantCulture));
        if (numberInt == 0)
        {
            dmgNumber.size = 0.75f;
            dmgNumber.textMesh.color = prefab.nullColor;
        }
        else if (numberInt <= prefab.smallThreshold)
        {
            dmgNumber.size = 0.75f;
            dmgNumber.textMesh.color = prefab.smallColor;
        }
        else if (numberInt >= prefab.critThreshold)
        {
            dmgNumber.size = 1.5f;
            dmgNumber.textMesh.color = prefab.criticalColor;
        }
        else
            dmgNumber.textMesh.color =  prefab.normalColor;
        dmgNumber.transform.localScale = Vector3.zero;
    }
}
