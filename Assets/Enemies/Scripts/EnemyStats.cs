using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] GameColor color;
    [SerializeField] int colorAmmount;
    private GameColor comboColor;

    public void DamageEnemy(float damage)
    {
        health -= damage;
        if(health <= 0) KillEnemy();
    }

    public void KillEnemy()
    {
        //TODO
        Debug.Log(gameObject.name + " died. Enemy deaths not implemented");
    }

    public (GameColor color, int ammount) AbsorbColor()
    {
        GameColor ret = color;
        int ammount = colorAmmount;
        if (ammount == 0) ret = null;
        color = null;
        ammount = 0;
        return (ret, ammount);
    }

    public GameColor CheckColor()
    {
        return color;
    }

    /* TODO
    public void AddComboColor(GameColor combo)
    {
        combo.MixColor(comboColor);
        //TODO reset if brown
    }
    */
}
