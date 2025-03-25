using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/On Damage/Temporary Bandaid")]
public class TemporaryBandaid : CustomItem
{
    [SerializeField] string itemName;
    [SerializeField] int shield;
    [SerializeField] int shieldPerStack;

    public override void AddEffect()
    {
        PlayerStats player = GameObject.Find("Player").GetComponent<PlayerStats>();
        if (player.itemVaribles.ContainsKey(itemName))
        {
            player.itemVaribles[itemName]++;
        } else
        {
            player.itemVaribles.Add(itemName, 1);
            player.onPlayerRealDamage += Effect;
        }
    }

    public override void Effect(PlayerStats player, EnemyStats hit)
    {
        Debug.Log("here");
        player.AddShield(shield + player.itemVaribles[itemName] * shieldPerStack);
    }

    public override void RemoveEffect()
    {
        PlayerStats player = GameObject.Find("Player").GetComponent<PlayerStats>();
        player.itemVaribles[itemName]--;
        if (player.itemVaribles[itemName] == 0) GameObject.Find("Player").GetComponent<PlayerStats>().onPlayerRealDamage -= Effect;
    }
}
