using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if an impact with the ground has happened
/// </summary>
public class ColorSpellGroundedDec : SpellImpact
{
    /// <summary>
    /// Forwards the impact to the refer coponent
    /// </summary>
    [SerializeField] SpellImpact refer;

    public override void Init(ColorSpell spell)
    {
        base.Init(spell);
        refer.Init(spell);
    }

    public override void Impact(Collider2D other)
    {
        bool test = Physics2D.Raycast(transform.position , Vector2.down, 1f, ~LayerMask.GetMask("Enemy", "Player", "Spell", "Item"));
        if (test)
        {
            refer.Impact(other);
        }
    }
}
