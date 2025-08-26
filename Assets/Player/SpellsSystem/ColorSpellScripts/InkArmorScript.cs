using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkArmorScript : SpellImpact
{
    [SerializeField] bool isSubSpell;
    [SerializeField] GameObject[] spawnPrefabs;
    [SerializeField] GameObject[] spawnPrefabsMiss;

    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        if (isSubSpell) return;
        Player.instance.GetComponent<PlayerStats>().AddInkArmor(this);
    }

    public void DisableInkArmor()
    {
        OnDisable();
    }

    private void OnDisable()
    {
        Player.instance.GetComponent<PlayerStats>().RemoveInkArmor(this);
    }

    public void InitiateArmor(EnemyStats enemy)
    {
        if (isSubSpell) return;
        if (enemy != null)
        {
            foreach (GameObject spawnPrefab in spawnPrefabs)
            {
                GameObject obj = GameObject.Instantiate(spawnPrefab, enemy.transform.position, transform.rotation) as GameObject;
                obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir, spell.GetExtraDamage());
                foreach (SpellEnemyInteraction enemyInteraction in obj.GetComponents<SpellEnemyInteraction>())
                {
                    enemyInteraction.SetEnemy(enemy.GetComponent<Collider2D>());
                }
            }
        } else
        {
            foreach (GameObject spawnPrefab in spawnPrefabsMiss)
            {
                GameObject obj = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
                obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir, spell.GetExtraDamage());
            }
        }
        Destroy(gameObject);
    }
}
