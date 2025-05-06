using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTintedStoneManager : SpellImpact
{
    [SerializeField] float floatingSpeed = 1;
    [SerializeField] float rngMin, rngMax;
    [SerializeField] float attackForce;
    [SerializeField] string spellKey;
    [SerializeField] LayerMask layerMask;
    [SerializeField] CircleCollider2D trigger;
    [SerializeField] GameObject[] spawnPrefabs;
    [SerializeField] Transform[] anchorPoints;

    List<GameObject> subSpells = new List<GameObject>();
    Dictionary<GameObject, Transform> anchorDictionary = new Dictionary<GameObject, Transform>();
    Dictionary<GameObject, float> randomSpeeds = new Dictionary<GameObject, float>();


    private void Start()
    {
        spell.GetPlayerObj().GetComponent<PlayerCombatSystem>().onRecast += ReCast;
        for (int i = 0; i < spawnPrefabs.Length; i++)
        {
            Transform anchor = anchorPoints[i % anchorPoints.Length];

            GameObject obj = GameObject.Instantiate(spawnPrefabs[i], transform.position, anchor.transform.rotation) as GameObject;
            obj.GetComponent<ColorSpell>().Initi(spell.GetColor(), spell.GetPower(), spell.GetPlayerObj(), spell.lookDir, spell.GetExtraDamage());
            subSpells.Add(obj);
            anchorDictionary.Add(obj, anchor);
            randomSpeeds.Add(obj, floatingSpeed + Random.Range(rngMin, rngMax));
        }
    }

    private void OnDisable()
    {
        spell.GetPlayerObj().GetComponent<PlayerCombatSystem>().onRecast -= ReCast;
    }

    private void FixedUpdate()
    {
        foreach (GameObject obj in subSpells)
        {
            Transform anchor = anchorDictionary[obj];
            float speed = randomSpeeds[obj];
            obj.transform.position += ((anchor.position) - obj.transform.position).normalized * speed * Time.fixedDeltaTime * Vector3.Distance(anchor.position, obj.transform.position);
        }
    }
    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        if (subSpells.Count <= 0)
        {
            Destroy(gameObject);
            return;
        }


        Vector2 dir = other.transform.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, trigger.radius, layerMask);
        if (hit.transform != other.transform) return;

        GameObject obj = subSpells[0];
        subSpells.RemoveAt(0);
        if (obj == null) return;
        obj.GetComponent<Rigidbody2D>().AddForce((other.transform.position - obj.transform.position).normalized * attackForce);
        Destroy(obj, 2);
        anchorDictionary.Remove(obj);
        if (subSpells.Count == 0) Destroy(gameObject);
    }

    public void ReCast(string key)
    {
        if (key.Equals(spellKey))
        {
            foreach (GameObject obj in subSpells)
            {
                obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(spell.GetPlayerObj().GetComponent<PlayerMovement>().lookDir, Random.Range(-0.15f,0.01f)).normalized * attackForce);
                obj.GetComponent<ColorSpell>().destroyOnAllImpact = true;
                obj.GetComponent<ColorSpell>().impactOnNonEnemies = true;
                Destroy(obj, 0.5f);
            }
            subSpells.Clear();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (subSpells.Count > 0) ReCast(spellKey);
    }
}
