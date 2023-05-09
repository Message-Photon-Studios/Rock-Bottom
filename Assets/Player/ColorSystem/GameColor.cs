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
    /// A description of the color
    /// </summary>
    [SerializeField, TextArea(5,20)] public string description;

    /// <summary>
    /// The effect that this color has
    /// </summary>
    [SerializeField] ColorEffect colorEffect;
    [SerializeField] List<ColorMix> mixes;
    
    /// <summary>
    /// Returns the mix of this color with the specified color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public GameColor MixColor(GameColor color)
    {
        if(mixes.Exists(item => item.mixWith == color))
        {
            return mixes.Find(item => item.mixWith == color).mixTo;
        }

        else return this;
    }

    public void ApplyColorEffect(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power)
    {
        if(enemyObj.GetComponent<EnemyStats>().GetColor() == this)
        {
            enemyObj.GetComponent<EnemyStats>().DamageEnemy(0);
            return;
        }
        colorEffect.Apply(enemyObj, impactPoint, playerObj, power);
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
