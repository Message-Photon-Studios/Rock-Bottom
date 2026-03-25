using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorSpell : SpellImpact
{
    [SerializeField] float speed;
    public static MirrorSpell mirrorSource { get; private set; }
    public int slotIndex { get; set; }
    static int position = 0;
    static Vector3 anchor;
    bool ready = false;

    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        Player.instance.playerCombatSystem.onSpellCast += InvokeCopy;
        GetComponent<ColorSpell>().onSpellIniti += SpellIniti;
    }

    private void SpellIniti()
    {
        SetMirrorSource(this);
        ready = true;
        GetComponent<ColorSpell>().onSpellIniti -= SpellIniti;
    }

    private void OnDisable()
    {
        Player.instance.playerCombatSystem.onSpellCast -= InvokeCopy;
        GetComponent<ColorSpell>().onSpellIniti -= SpellIniti;
    }

    static void SetMirrorSource(MirrorSpell newSource)
    {
        if (!mirrorSource)   mirrorSource = newSource;
        else
        {
            mirrorSource.GetComponent<ColorSpell>().Initi(newSource.spell.GetColor(), newSource.spell.GetPower(), newSource.spell.GetPlayerObj(), newSource.spell.lookDir, newSource.spell.GetExtraDamage());
            Destroy(newSource.gameObject);
        }
        position = (position + 1) % 4;
        if (position == 1) anchor = new Vector3(2, 0, 0); 
        if (position == 2) anchor = new Vector3(0, 2, 0); 
        if (position == 3) anchor = new Vector3(-2, 0, 0);
        if (position == 0) anchor = new Vector3(0, -2, 0);
    }

    //static IEnumerable DestroySpell()


    private void FixedUpdate()
    {
        if (!ready) return;
        int lookDir = (int) Player.instance?.playerMovement.lookDir;
        Vector3 localAnchor = (Vector3)Player.instance?.transform.position + new Vector3(anchor.x * lookDir, anchor.y);
        transform.position += (localAnchor - transform.position).normalized * speed * Time.fixedDeltaTime * Vector3.Distance(localAnchor, transform.position);
    }

    void InvokeCopy(float power, ColorSpell inSpell, GameColor color)
    {
        if (!ready) return;
        if (inSpell.isCopy || inSpell.castCopy || !inSpell.canBeCopied) return;
        GameObject obj = GameObject.Instantiate(inSpell.gameObject, transform.position, transform.rotation) as GameObject;
        int invert = 1;
        if (position == 3) invert = -1;
        obj.GetComponent<ColorSpell>().InitiCopy(spell.GetColor(), spell.GetPower(), gameObject, Player.instance.playerMovement.lookDir * invert, spell.GetExtraDamage());
    }
}
