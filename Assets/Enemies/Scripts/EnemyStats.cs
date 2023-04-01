using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Important stats for an enemy.
/// </summary>
public class EnemyStats : MonoBehaviour
{
    [SerializeField] float health; //The health of the enemy
    [SerializeField] GameColor color; //The color of the enemy
    [SerializeField] int colorAmmount; //The ammount of color you will get when absorbing the color from the enemy
    [SerializeField] float movementSpeed; //The current movement speed of the enemy
    
    private float normalMovementSpeed; //The normal movement speed of the enemy
    private float movementSpeedTimer; 

    private float comboTime = 1; //The timelimit for the next move of a combo
    private float comboTimer = 0;
    private GameColor comboColor; //The color that currently affects the enemy in a combo
    
    #region Setup and Timers
    void OnEnable()
    {
        normalMovementSpeed = movementSpeed;
    }

    void Update()
    {
        if(comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if(comboTimer <= 0) 
            {
                comboTimer = 0;
                comboColor = null;
            }
        }

        if(movementSpeedTimer > 0)
        {
            movementSpeedTimer -= Time.deltaTime;
            if(movementSpeedTimer <= 0)
            {
                movementSpeedTimer = 0;
                movementSpeed = normalMovementSpeed;
            }
        }
    }

    #endregion

    #region Damage and Kill
    /// <summary>
    /// Damage the enemy with the specified damage
    /// </summary>
    /// <param name="damage"></param>
    public void DamageEnemy(float damage)
    {
        health -= damage;
        if(health <= 0) KillEnemy();
    }

    /// <summary>
    /// Kill then enemy
    /// </summary>
    public void KillEnemy()
    {
        //TODO
        Debug.Log(gameObject.name + " died. Enemy deaths not implemented");
    }

    /// <summary>
    /// Returns the enemys current health
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }

    #endregion

    #region Enemyy Color

    /// <summary>
    /// Return what color this enemy has and how much, then remove the color form the enemy.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="AbsorbColor("></param>
    /// <returns></returns>
    public (GameColor color, int ammount) AbsorbColor()
    {
        GameColor ret = color;
        int ammount = colorAmmount;
        if (colorAmmount == 0) ret = null;
        color = null;
        colorAmmount = 0;
        return (ret, ammount);
    }

    /// <summary>
    /// Check what color this enemy has
    /// </summary>
    /// <returns></returns>
    public GameColor GetColor()
    {
        return color;
    }

    #endregion

    #region Combo Color

    /// <summary>
    /// Sets the combo color for the player
    /// </summary>
    /// <param name="comboColor"></param>
    public void SetComboColor(GameColor comboColor)
    {
        this.comboColor = comboColor;
        comboTimer = comboTime;
    }

    /// <summary>
    /// Get the current combo color of the player
    /// </summary>
    /// <returns></returns>
    public GameColor GetComboColor()
    {
        return comboColor;
    }

    #endregion

    #region Movement Speed

    /// <summary>
    /// Change the enemys movement speed for a certain amount of time
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="time"></param>
    public void ChangeSpeed (float speed, float time)
    {
        movementSpeed = speed;
        movementSpeedTimer = time;
    }

    /// <summary>
    /// Returns the enemys current speed
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return movementSpeed;
    }

    #endregion
}
