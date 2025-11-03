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
    [SerializeField, TextArea(5, 20)] public string description;

    /// <summary>
    /// The effect that this color has
    /// </summary>
    [SerializeField] ColorEffect colorEffect;
    [SerializeField] List<ColorMix> mixes;
    [SerializeField] public GameColor[] rootColors;

    //Icon representing the color.
    [SerializeField] public Sprite colorIcon;

    [SerializeField] private bool canColorEnemies = true;
    
    /// <summary>
    /// Returns the mix of this color with the specified color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public GameColor MixColor(GameColor color)
    {
        if (color != null && mixes.Exists(item => item.mixWith == color))
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
    
    /// <summary>
    /// Returns true if this color and the other color shares at least one root color.
    /// </summary>
    /// <param name="otherColor"></param>
    /// <returns></returns>
    public bool SharesRootColor(GameColor otherColor)
    {
        foreach(GameColor rootColor in otherColor.rootColors)
        {
            if(ContainsRootColor(rootColor)) return true;
        }

        return false;
    } 

    /// <summary>
    /// Returns a mixed color of all shared root colors between this color and the other color.
    /// </summary>
    /// <param name="otherColor"></param>
    /// <returns></returns>
    public GameColor SharedRootMix(GameColor otherColor)
    {
        if(!SharesRootColor(otherColor)) return null;

        GameColor mix = null;
        
        foreach (GameColor rootColor in otherColor.rootColors)
        {
            if(ContainsRootColor(rootColor))
            {
                if(mix == null) mix = rootColor;
                else mix = mix.MixColor(rootColor);
            }
        }

        return mix;
    }

    /// <summary>
    /// Returns the remaining color after you have removed the subtracting color from it.
    /// </summary>
    /// <param name="subtractingColor"></param>
    /// <returns></returns>
    public GameColor ColorSubtraction(GameColor subtractingColor)
    {
        if(!SharesRootColor(subtractingColor)) return this;

        GameColor mix = Player.instance.colorInventory.GetEmptyBottleColor();
        foreach (GameColor rootColor in rootColors)
        {
            if(!subtractingColor.ContainsRootColor(rootColor))
            {
                mix = mix.MixColor(rootColor);
            }
        }

        return mix;
    }
    public void ApplyColorEffect(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float colorPower, bool forcePerspectivePlayer, int extraDamage)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        PlayerStats playerStats = playerObj.GetComponent<PlayerStats>();
        float powerScale = 1;
        
        /* #Removed this because we don't think it is needed anymore
        if (enemy.GetColor() == this && !playerStats.corrosiveColor)
        {
            powerScale = .75f;
            GameManager.instance.tipsManager.DisplayTips("colorImmunity");
        }
        */

        if(playerStats.corrosiveColor)
        {   
            if(enemy.GetColor() == this)
                powerScale = 1.5f;
            else 
                powerScale = 0.75f;
        }

        /*
        if(enemy.GetColor() == null || enemy.GetColorAmmount() <= 0) 
        {
            powerScale = 0.75f;
            GameManager.instance.soundEffectManager.PlaySound(name, .25f);
        }*/
        
        GameManager.instance.soundEffectManager.PlaySound(name);

        if (GameManager.instance.GetComponent<ColorLibrary>().IsComplemtarty(enemy.GetColor(), this)) extraDamage += playerStats.complimentaryDamage;

        GameColor setToColor = enemy.GetPlayerMixColor(this);

        if (canColorEnemies) enemy.SetPlayerColor(setToColor, 1);

        colorPower += enemyObj.GetComponent<EnemyStats>().GetSleepPowerBonus();
        colorPower = colorPower * powerScale;

        if (Player.instance.colorInventory.colorVarnish && setToColor != this)
        {
            int colorDamage = enemy.GetColorAmmount();
            if (colorDamage > 20) colorDamage = 20;
            extraDamage += colorDamage;
        }

        if (Player.instance.colorInventory.greatBrushFirstHit)
        {
            if (enemy.GetHealth() / (float)enemy.GetMaxHealth() > .9f)
            {
                extraDamage += 15;
            }

            extraDamage = Mathf.RoundToInt(extraDamage * colorPower);
        }
        if (Player.instance.colorInventory.doubleExtraDamage) extraDamage *= 2;

        colorEffect.Apply(enemyObj, impactPoint, playerObj, colorPower, forcePerspectivePlayer, extraDamage);

        if (!canColorEnemies) return;
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Random.Range(0, 100) < playerStats.chanceToColorNearby)
            {
                EnemyStats objStats = obj.GetComponent<EnemyStats>();
                //if(objStats.GetColor() != null) return;               
                if (obj != enemy.gameObject && Vector2.Distance(obj.transform.position, enemy.transform.position) < playerStats.colorNearbyRange)
                {
                    GameColor setObjColor = objStats.GetPlayerMixColor(this);
                    if (setObjColor.canColorEnemies) objStats.SetPlayerColor(setObjColor, 1);
                }
            }
        }


    }

    public void MixThisColorOntoEnemy(EnemyStats enemy, PlayerStats playerStats)
    {
        if (!canColorEnemies) return;
        
        GameColor setToColor = enemy.GetPlayerMixColor(this);
        enemy.SetPlayerColor(setToColor, 1);

        if (playerStats.chanceToColorNearby <= 0) return;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Random.Range(0, 100) < playerStats.chanceToColorNearby)
            {
                EnemyStats objStats = obj.GetComponent<EnemyStats>();             
                if (obj != enemy.gameObject && Vector2.Distance(obj.transform.position, enemy.transform.position) < playerStats.colorNearbyRange)
                {
                    GameColor setObjColor = objStats.GetPlayerMixColor(this);
                    if(setObjColor.canColorEnemies) objStats.SetPlayerColor(setObjColor, 1);
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
