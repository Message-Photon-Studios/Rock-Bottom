using UnityEngine;

/// <summary>
/// Effects all the impacted list with the color effect
/// </summary>
public class ColorSpellImpact : SpellImpact
{

    [SerializeField] public ParticleSystem onImpactParticles;

    public override void Impact(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            RaycastHit2D test = Physics2D.Raycast(transform.position, other.transform.position-transform.position,Vector2.Distance(other.transform.position, transform.position)-.1f, ~LayerMask.GetMask("Spell", "Player", "Ignore Raycast", "Item", "Enemy"));
            if(test.collider != null) 
                return;

            EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();

            if (enemy != null)
            {
                spell.GetColor().ApplyColorEffect(other.gameObject, transform.position, spell.GetPlayerObj(), spell.GetPower());
                enemy.enemySounds?.PlaySpellHit();
            }
        }

        var instantiatedParticles = GameObject.Instantiate(onImpactParticles, transform.position, transform.rotation);
        // Change the particle color to the color of the spell
        var main = instantiatedParticles.main;
        main.startColor = spell.GetColor().plainColor;
        instantiatedParticles.Play();
        Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration * 2);
    }
}
