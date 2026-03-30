using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/On Damage/Wind Bag")]
public class WindBag : CustomItem
{
    [SerializeField] string itemName;
    [SerializeField] float range;
    [SerializeField] float rangePerStack;
    [SerializeField] float force;
    [SerializeField] float forcePerStack;
    [SerializeField] BlueColorEffect colorEffect;

    public override void AddEffect()
    {
        PlayerStats player = GameObject.Find("Player").GetComponent<PlayerStats>();
        if (player.itemVaribles.ContainsKey(itemName))
        {
            player.itemVaribles[itemName]++;
        }
        else
        {
            player.itemVaribles.Add(itemName, 1);
            player.onPlayerDamaged += Effect;
        }
    }

    public override void Effect(PlayerStats player, EnemyStats hit)
    {
        if(hit == null) return;
        EnemyStats[] enemies = FindObjectsOfType<EnemyStats>();
        int count = player.itemVaribles[itemName];
        float currentRange = range + rangePerStack * count;
        foreach (EnemyStats enemy in enemies)
        {
            float distance = (enemy.transform.position - player.transform.position).sqrMagnitude;
            if ((distance < Mathf.Pow(currentRange , 2) || enemy == hit) && !enemy.IsKnockbackImune())
            {
                colorEffect.Apply(enemy.gameObject, Player.instance.transform.position, Player.instance.gameObject, (force + forcePerStack * count), true, 0, false);
                //enemy.GetComponent<Rigidbody2D>().AddForce(((enemy.transform.position - player.transform.position).normalized + new Vector3(0,1,0)).normalized * (force + forcePerStack* count));
            }
        }
    }

    public override void RemoveEffect()
    {
        PlayerStats player = GameObject.Find("Player").GetComponent<PlayerStats>();
        player.itemVaribles[itemName]--;
        if (player.itemVaribles[itemName] == 0) GameObject.Find("Player").GetComponent<PlayerStats>().onPlayerDamaged -= Effect;
    }
}
