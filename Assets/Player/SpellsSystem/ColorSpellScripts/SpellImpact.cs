using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellImpact : MonoBehaviour
{

    protected ColorSpell spell;
    public virtual void Init(ColorSpell spell) { this.spell = spell; }
    public abstract void Impact(Collider2D other, Vector2 impactPoint);
}
