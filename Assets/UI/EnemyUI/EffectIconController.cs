using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using System;
using Unity.VisualScripting;

public class EffectIconController : MonoBehaviour
{
    [SerializeField] SerializedDictionary<EffectIcon, Sprite> effectIconDictionary;
    private int iconIndex = 0;
    private Image[] iconImages;
    private Dictionary<EffectIcon, int> iconPositions = new Dictionary<EffectIcon, int>();

    private EnemyStats enemy;
 
    // Start is called before the first frame update
    void Start()
    {
        iconPositions = new Dictionary<EffectIcon, int>();
        iconImages = GetComponentsInChildren<Image>();
        foreach (var icon in iconImages)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }
        enemy = GetComponentInParent<EnemyHpController>().enemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.IsAsleep() && !enemy.HasSleepCooldown())
            AddIcon(EffectIcon.Sleep);
        else
            removeIcon(EffectIcon.Sleep);
        
        if(enemy.HasSleepCooldown())
            AddIcon(EffectIcon.SleepBlock);
        else
            removeIcon(EffectIcon.SleepBlock);

        if (enemy.isBurning())
            AddIcon(EffectIcon.Burning);
        else
            removeIcon(EffectIcon.Burning);

        if (enemy.isFrozen())
            AddIcon(EffectIcon.Frozen);
        else
            removeIcon(EffectIcon.Frozen);

        if (enemy.isPoisoned())
            AddIcon(EffectIcon.Poison);
        else
            removeIcon(EffectIcon.Poison);

        if(enemy.IsRaibowed())
            AddIcon(EffectIcon.Rainbow);
        else
            removeIcon(EffectIcon.Rainbow);
    }

    void AddIcon(EffectIcon icon)
    {
        if(iconPositions.ContainsKey(icon)) return;
        if(!effectIconDictionary.ContainsKey(icon))
        {
            Debug.LogError(enemy.name + " is missing effect icon for " + icon.ToString());
            return;
        }

        iconPositions[icon] = iconIndex;
        iconImages[iconIndex].sprite = effectIconDictionary[icon];
        iconIndex ++;
        foreach (var icnObj in iconImages) icnObj.gameObject.SetActive(icnObj.sprite != null);
    }

    void removeIcon(EffectIcon icon)
    {
        if (iconPositions == null || !iconPositions.ContainsKey(icon)) return;
        int pos = iconPositions[icon];
        iconImages[pos].sprite = null;
        for (int i = pos; i < iconImages.Length-1; i++)
        {
            iconImages[i].sprite = iconImages[i + 1].sprite;
        }
        List<EffectIcon> toChange = new List<EffectIcon>();
        foreach(KeyValuePair<EffectIcon, int> iconPosition in iconPositions)
        {
            if(iconPosition.Value >= pos) toChange.Add(iconPosition.Key);
        }

        for (int i = 0; i < toChange.Count; i++)
        {
            iconPositions[toChange[i]] --;
        }

        iconPositions.Remove(icon);
        iconIndex--;
        foreach (var icnObj in iconImages) icnObj.gameObject.SetActive(icnObj.sprite != null);
    }
}

[System.Serializable]
enum EffectIcon
{
    Sleep,
    SleepBlock,
    Poison, 
    Burning,
    Frozen,
    Rainbow
}