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
    [SerializeField] SpellImpact success;
    [SerializeField] SpellImpact fail;
    [SerializeField] float radius = 0;

    public override void Init(ColorSpell spell)
    {
        base.Init(spell);
        success.Init(spell);
        if (fail != null) fail.Init(spell);
    }

    public override void Impact(Collider2D other)
    {
        bool test = Physics2D.Raycast((Vector2) transform.position + Vector2.right*radius, Vector2.down, 1.5f, ~LayerMask.GetMask("Enemy", "Player", "Spell", "Item", "OnlyHitGround")) || 
            Physics2D.Raycast((Vector2)transform.position - Vector2.right * radius, Vector2.down, 1.5f, ~LayerMask.GetMask("Enemy", "Player", "Spell", "Item", "OnlyHitGround"));
        if (test)
        {
            success.Impact(other);
        } else if (fail != null)
        {
            fail.Impact(other);
        }
    }
}
