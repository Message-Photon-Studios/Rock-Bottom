using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/On Damage/Parry Brush")]
public class ParryBrush : CustomItem
{
    [SerializeField] string itemName;
    [SerializeField] int damage;
    [SerializeField] int damagePerStack;
    [SerializeField] float power;
    [SerializeField] float powerPerStack;
    [SerializeField] float range;
    [SerializeField] YellowColorEffect colorEffect;

    public override void AddEffect()
    {
        PlayerStats player = Player.instance.playerStats;
        if (player.itemVaribles.ContainsKey(itemName))
        {
            player.itemVaribles[itemName]++;
        } else
        {
            player.itemVaribles.Add(itemName, 1);
            player.onPlayerDamaged += Effect;
        }
    }

    public override void Effect(PlayerStats player, EnemyStats hit)
    {
        if (hit == null)
        {
            float shortestDistanse = Mathf.Pow(range, 2);
            EnemyStats[] enemies = FindObjectsOfType<EnemyStats>();
            foreach (EnemyStats enemy in enemies)
            {
                float distance = (enemy.transform.position - player.transform.position).sqrMagnitude;
                if (distance < shortestDistanse) shortestDistanse = distance; hit = enemy;
            }
        }
        
        if (hit == null) return;
        //hit.DamageEnemy(damage + player.itemVaribles[itemName] * damagePerStack);
        colorEffect.Apply(hit.gameObject, hit.transform.position, Player.instance.gameObject, (power + player.itemVaribles[itemName] * powerPerStack), true, (damage + player.itemVaribles[itemName] * damagePerStack), true);
    }

    public override void RemoveEffect()
    {
        PlayerStats player = GameObject.Find("Player").GetComponent<PlayerStats>();
        player.itemVaribles[itemName]--;
        if (player.itemVaribles[itemName] == 0) GameObject.Find("Player").GetComponent<PlayerStats>().onPlayerDamaged -= Effect;
    }
}
