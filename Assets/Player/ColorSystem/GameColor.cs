using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Color/GameColor", menuName = "Gameplay Color/GameColor")]
public class GameColor : ScriptableObject
{
    /// <summary>
    /// The actual color shader of the GameColor
    /// </summary>
    [SerializeField] public Material colorMat;

    /// <summary>
    /// The color of the GameColor without taking bloom into account
    /// </summary>
    [SerializeField] public Color plainColor;

    /// <summary>
    /// How this color should tint lights
    /// </summary>
    [SerializeField] public Color lightTintColor;

    /// <summary>
    /// A description of the color
    /// </summary>
    [SerializeField, TextArea(5,20)] public string description;

    /// <summary>
    /// The effect that this color has
    /// </summary>
    [SerializeField] ColorEffect colorEffect;
    [SerializeField] List<ColorMix> mixes;
    [SerializeField] public GameColor[] rootColors;

    //Icon representing the color.
    [SerializeField] public Sprite colorIcon;
    
    /// <summary>
    /// Returns the mix of this color with the specified color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public GameColor MixColor(GameColor color)
    {
        if(color != null && mixes.Exists(item => item.mixWith == color))
        {
            return mixes.Find(item => item.mixWith == color).mixTo;
        }

        else return this;
    }

    /// <summary>
    /// Returns true if this color contains the root color
    /// </summary>
    /// <param name="rootColor"></param>
    /// <returns></returns>
    public bool ContainsRootColor(GameColor rootColor)
    {
        for (int i = 0; i < rootColors.Length; i++)
        {
            if(rootColors[i] == rootColor) return true;
        }
        
        return false;
    }
    public void ApplyColorEffect(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats playerStats = playerObj.GetComponent<PlayerStats>();
        
        if (enemy.GetColor() == this && !playerStats.corrosiveColor)
        {
            enemy.DamageEnemy(0);
            GameManager.instance.tipsManager.DisplayTips("colorImmunity");
            return;
        }

        float powerDivide = 1;
        if (playerStats.corrosiveColor && enemy.GetColor() != this) powerDivide = 1.333f;
        bool setPowerZero = false;
        if(enemy.GetColor() == null || enemy.GetColorAmmount() <= 0) 
        {
            GameManager.instance.tipsManager.DisplayTips("uncoloredDefense");
            powerDivide = 2;
            GameManager.instance.soundEffectManager.PlaySound(name, .25f);
        } else
            GameManager.instance.soundEffectManager.PlaySound(name);

        if (GameManager.instance.GetComponent<ColorLibrary>().IsComplemtarty(enemy.GetColor(), this)) extraDamage += playerStats.complimentaryDamage;
        
        GameColor setToColor = (Random.Range(0,100) < playerStats.chanceThatEnemyDontMix)?this:MixColor(enemy.GetColor());

        bool delay = setToColor.name.Equals("Rainbow");

        if (delay) enemy.SetColor(setToColor, enemy.GetColorAmmount() + 1);

        power += enemyObj.GetComponent<EnemyStats>().GetSleepPowerBonus();
        power = power / powerDivide;
        if(setPowerZero) power = 0;
        colorEffect.Apply(enemyObj, impactPoint, playerObj, power, forcePerspectivePlayer, extraDamage);

        if (!delay) enemy.SetColor(setToColor, enemy.GetColorAmmount() + 1);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(Random.Range(0, 100) < playerStats.chanceToColorNearby)
            {
                EnemyStats objStats = obj.GetComponent<EnemyStats>();
                    //if(objStats.GetColor() != null) return;               
                if(obj != enemy.gameObject && Vector2.Distance(obj.transform.position, enemy.transform.position) < playerStats.colorNearbyRange)
                {
                    GameColor setObjColor = (Random.Range(0,100) < playerStats.chanceThatEnemyDontMix)?this:MixColor(objStats.GetColor());
                    objStats.SetColor(setObjColor, objStats.GetColorAmmount() + 1);
                }
            }
        }


    }

    public void MixThisColorOntoEnemy(EnemyStats enemy, PlayerStats playerStats)
    {
        GameColor setToColor = (Random.Range(0, 100) < playerStats.chanceThatEnemyDontMix) ? this : MixColor(enemy.GetColor());
        enemy.SetColor(setToColor, enemy.GetColorAmmount() + 1);

        if (playerStats.chanceToColorNearby <= 0) return;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Random.Range(0, 100) < playerStats.chanceToColorNearby)
            {
                EnemyStats objStats = obj.GetComponent<EnemyStats>();             
                if (obj != enemy.gameObject && Vector2.Distance(obj.transform.position, enemy.transform.position) < playerStats.colorNearbyRange)
                {
                    GameColor setObjColor = (Random.Range(0, 100) < playerStats.chanceThatEnemyDontMix) ? this : MixColor(objStats.GetColor());
                    objStats.SetColor(setObjColor, objStats.GetColorAmmount() + 1);
                }
            }
        }
    }

    /// <summary>
    /// Returns the color effect of this color
    /// </summary>
    /// <returns></returns>
    public ColorEffect GetColorEffect() { return colorEffect; }
}

[System.Serializable]
public struct ColorMix
{
    [SerializeField] public GameColor mixWith;
    [SerializeField] public GameColor mixTo;
}
