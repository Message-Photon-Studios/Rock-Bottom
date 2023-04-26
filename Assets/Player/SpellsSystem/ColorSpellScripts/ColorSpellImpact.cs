using UnityEngine;

/// <summary>
/// Effects all the impacted enemies with the color effect
/// </summary>
public class ColorSpellImpact : SpellImpact
{

    [SerializeField] public ParticleSystem onImpactParticles;

    public override void Impact(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            GameObject test = Physics2D.Raycast(transform.position, other.transform.position-transform.position,1000, ~LayerMask.GetMask("Spell", "Player")).collider.gameObject;
            if(test.name != other.name) 
                return;

            EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();

            GameColor comboColor = spell.GetColor().MixColor(enemy.GetComboColor());

            enemy.SetComboColor(comboColor);
            

            if(comboColor != spell.GetColor())
            {
                enemy.currentCombo *= 2;
                if(enemy.currentCombo == 0) enemy.currentCombo = 1;

                if(comboColor.name == "Brown" ) 
                {
                    enemy.SetComboColor(null);
                    float comboDamage = spell.GetPlayerObj().GetComponent<PlayerCombatSystem>().comboBaseDamage * enemy.currentCombo;
                    enemy.currentCombo = 0;
                    enemy.DamageEnemy(comboDamage);
                }
            }


            if(enemy != null) spell.GetColor().ApplyColorEffect(other.gameObject, transform.position, spell.GetPlayerObj(), spell.GetPower());
        }

        var instantiatedParticles = GameObject.Instantiate(onImpactParticles, transform.position, transform.rotation);
        // Change the particle color to the color of the spell
        var main = instantiatedParticles.main;
        main.startColor = spell.GetColor().plainColor;
        instantiatedParticles.Play();
        Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration * 2);
    }
}
