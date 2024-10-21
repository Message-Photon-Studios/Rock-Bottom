using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Effects all the impacted list with the color effect
/// </summary>
public class ColorSpellImpact : SpellImpact
{
    /// <summary>
    /// Setting this variable to true will change the forces of some spells to refer to the player instead of the spells impact point
    /// </summary>
    [SerializeField] protected bool forcePerspectivePlayer;


    [SerializeField] public ParticleSystem onImpactParticles;

    public override void Impact(Collider2D other, Vector2 impactPoint)
    {
        if(other.CompareTag("Enemy"))
        {
            float collisionCheckDeadZone = .75f;
            if(Vector2.Distance(other.transform.position, transform.position) > collisionCheckDeadZone)
            {
                Vector2 spellPos = transform.position;
                Vector2 enemyPos = other.transform.position;
                Vector2 rayCastOrigin = spellPos + ((enemyPos-spellPos).normalized *collisionCheckDeadZone);
                Vector2 rayCastDirection = enemyPos-rayCastOrigin;
                float rayCastDistance = Vector2.Distance(enemyPos,rayCastOrigin);

                RaycastHit2D test = Physics2D.Raycast(rayCastOrigin, rayCastDirection, rayCastDistance, GameManager.instance.maskLibrary.onlyGround);
                if(test.collider != null) 
                {
                    return;
                }
            }

            EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();

            if (enemy != null)
            {
                spell.GetColor().ApplyColorEffect(other.gameObject, transform.position, spell.GetPlayerObj(), spell.GetPower(), forcePerspectivePlayer);
                enemy.enemySounds?.PlaySpellHit();
            }
        }

        var instantiatedParticles = GameObject.Instantiate(onImpactParticles, impactPoint, transform.rotation);
        // Change the particle color to the color of the spell
        var main = instantiatedParticles.main;
        main.startColor = spell.GetColor().plainColor;
        instantiatedParticles.Play();
        Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration * 2);
    }
}
