using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EffectIcon
{
    Burn,
    Freeze,
    Sleep,
    Poison
}

public class EffectIconController : MonoBehaviour
{
    [SerializeField] Sprite burnIcon;
    [SerializeField] Sprite freezeIcon;
    [SerializeField] Sprite sleepIcon;
    [SerializeField] Sprite poisonIcon;

    private int iconIndex = 0;
    private Image[] icons;
    private Dictionary<EffectIcon, int> iconPositions;

    private EnemyStats enemy;
 
    // Start is called before the first frame update
    void Start()
    {
        iconPositions = new Dictionary<EffectIcon, int>();
        icons = GetComponentsInChildren<Image>();
        foreach (var icon in icons)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }
        enemy = GetComponentInParent<EnemyHpController>().enemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.IsAsleep())
            addIcon(EffectIcon.Sleep);
        else
            removeIcon(EffectIcon.Sleep);

        if (enemy.isBurning())
            addIcon(EffectIcon.Burn);
        else
            removeIcon(EffectIcon.Burn);

        if (enemy.isFrozen())
            addIcon(EffectIcon.Freeze);
        else
            removeIcon(EffectIcon.Freeze);

        if (enemy.isPoisoned())
            addIcon(EffectIcon.Poison);
        else
            removeIcon(EffectIcon.Poison);
    }

    void addIcon(EffectIcon icon)
    {
        if (iconPositions.ContainsKey(icon)) return;
        icons[iconIndex].sprite = icon switch
        {
            EffectIcon.Burn => burnIcon,
            EffectIcon.Freeze => freezeIcon,
            EffectIcon.Sleep => sleepIcon,
            EffectIcon.Poison => poisonIcon,
            _ => icons[iconIndex].sprite
        };
        iconPositions[icon] = iconIndex++;
        foreach (var icnObj in icons) icnObj.gameObject.SetActive(icnObj.sprite != null);
    }

    void removeIcon(EffectIcon icon)
    {
        if (!iconPositions.ContainsKey(icon)) return;
        for (int i = iconPositions[icon]; i < 3; i++)
        {
            icons[i].sprite = icons[i + 1].sprite;
        }
        icons[3].sprite = null;
        iconPositions.Remove(icon);
        iconIndex--;
        foreach (var icnObj in icons) icnObj.gameObject.SetActive(icnObj.sprite != null);
    }
}
