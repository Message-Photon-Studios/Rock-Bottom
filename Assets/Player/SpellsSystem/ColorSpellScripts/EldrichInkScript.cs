using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldrichInkScript : SpellImpact
{
    [SerializeField] GameObject[] spawnPrefabs;
    [SerializeField] SpellImpact[] spellImpacts;

    [SerializeField] int playerDamage;
    [SerializeField] int playerHeal;
    [SerializeField] float startRange;
    [SerializeField] float activeRange;

    Dictionary<GameObject, GameObject> spawnedArms = new Dictionary<GameObject, GameObject>();

    private void Start()
    {
        foreach (SpellImpact impact in spellImpacts)
        {
            impact.Init(GetComponent<ColorSpell>());
        }
        Player.instance.GetComponent<PlayerStats>().DamagePlayer(playerDamage, null, spell.GetColor());
        GetComponent<CircleCollider2D>().radius = startRange;
        StartCoroutine(DisableTrigger());
    }

    private IEnumerator DisableTrigger()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<CircleCollider2D>().radius = activeRange;
    }

    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        if (spawnedArms.ContainsKey(other.gameObject)) return;
        foreach (GameObject spawn in spawnPrefabs)
        {
            GameObject obj = GameObject.Instantiate(spawn, transform.position, transform.rotation) as GameObject;
            obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir, spell.GetExtraDamage());
            obj.GetComponent<EldrichArmScript>().setEldrichParent(this);
            obj.GetComponent<EldrichArmScript>().setEnemyTarget(other.gameObject);
            if (!spawnedArms.ContainsKey(other.gameObject)) spawnedArms.Add(other.gameObject, obj);
        }
    }

    public void DetachEnemy(Collider2D other)
    {
        if (spawnedArms.ContainsKey(other.gameObject))
        {
            spawnedArms.Remove(other.gameObject);
        }
    }

    public bool ReAttachEnemy(GameObject enemy, GameObject arm)
    {
        if (spawnedArms.ContainsKey(enemy)) return false;
        spawnedArms.Add(enemy, arm);
        arm.GetComponent<EldrichArmScript>().setEnemyTarget(enemy);
        return true;
    }

    private void OnDestroy()
    {
        foreach(GameObject enemy in spawnedArms.Keys)
        {
            spawnedArms[enemy].GetComponent<EldrichArmScript>().DisableArm();
            foreach (SpellImpact impact in spellImpacts)
            {
                impact.Impact(enemy.GetComponent<Collider2D>(), transform.position);
            }
            Player.instance.GetComponent<PlayerStats>().HealPlayer(playerHeal);
        }
    }
}
