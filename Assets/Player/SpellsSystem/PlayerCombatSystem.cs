using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// This class handles the players attack actions and spawn the color spells
/// </summary>
public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] float defaultAttackDamage;
    [SerializeField] float defaultAttackForce;
    [SerializeField] public float comboBaseDamage;
    [SerializeField] Transform spellSpawnPoint; //The spawn point for the spells. This will be automatically fliped on the x-level
    [SerializeField] PlayerDefaultAttack defaultAttackHitbox; //The object that controlls the default attack hitbox
    [SerializeField] Vector2 defaultAttackOffset; //The offset that the default attack will be set to
    [SerializeField] InputActionReference defaultAttackAction, specialAttackAction, verticalLookDir;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] Animator animator;
    [SerializeField] PlayerSounds playerSounds;
    private bool attacking;
    private Rigidbody2D body;

    Action<InputAction.CallbackContext> specialAttackHandler;
    Action<InputAction.CallbackContext> defaultAttackHandler;


    #region Setup
    private void OnEnable() {
        specialAttackHandler = (InputAction.CallbackContext ctx) => SpecialAttackAnimation();
        defaultAttackHandler = (InputAction.CallbackContext ctx) => {
            if(animator.GetBool("grapple")) return;
            if(attacking) return;
            attacking = true;
            animator.SetTrigger("defaultAttack");
            body.constraints |= RigidbodyConstraints2D.FreezePositionY;
            playerMovement.movementRoot.SetTotalRoot("attackRoot", true);
        };
        
        body = GetComponent<Rigidbody2D>();
        specialAttackAction.action.performed += specialAttackHandler;
        defaultAttackAction.action.performed += defaultAttackHandler;
        defaultAttackHitbox.onDefaultHit += EnemyHitDefault;
    }

    private void OnDisable()
    {
        specialAttackAction.action.performed -= specialAttackHandler;
        defaultAttackAction.action.performed -= defaultAttackHandler;
        defaultAttackHitbox.onDefaultHit -= EnemyHitDefault;
    }
    #endregion

    /// <summary>
    /// Handles the players default attack
    /// </summary>
    private void DefaultAttack()
    {
        Debug.Log("Default attack");
        //TODO add attacking = true;
        FlipDefaultAttack();
        defaultAttackHitbox.HitEnemies();
    }

    /// <summary>
    /// Flips the default attack
    /// </summary>
    public void FlipDefaultAttack()
    {
        float offsetX = defaultAttackOffset.x * playerMovement.lookDir;
        defaultAttackHitbox.transform.position = new Vector3(transform.position.x + offsetX, defaultAttackHitbox.transform.position.y, transform.position.z);
    }

    /// <summary>
    /// Is called when the player hits an enemy with the default attack
    /// </summary>
    /// <param name="enemyObj"></param>
    private void EnemyHitDefault(GameObject enemyObj)
    {
        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        (GameColor absorb, int ammount) = enemy.AbsorbColor();
        enemy.DamageEnemy(defaultAttackDamage);
        colorInventory.AddColor(absorb, ammount);
        enemy.GetComponent<Rigidbody2D>().AddForce(playerMovement.lookDir * Vector2.right * defaultAttackForce);
        enemy.enemySounds?.PlayOnHit();
    }

    private GameObject currentSpell = null;
    /// <summary>
    /// Plays the animation for the special attack
    /// </summary>
    private void SpecialAttackAnimation()
    {
        if(animator.GetBool("grapple")) return;
        currentSpell= colorInventory.GetActiveColorSpell().gameObject;
        if(currentSpell == null) return;
        if(attacking) return;
        if(!colorInventory.CheckActveColor()) return;
        
        attacking = true;
        string anim = currentSpell.GetComponent<ColorSpell>().GetAnimationTrigger();
        animator.SetTrigger(anim);
        playerMovement.movementRoot.SetTotalRoot("attackRoot", true);
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        playerSounds.PlayCastingSpell();
    }

    /// <summary>
    /// Handles the players special attack. Called by animation event
    /// </summary>
    private void SpecialAttack()
    {
        GameColor color = colorInventory.UseActiveColor();

        if(currentSpell == null) return;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x+currentSpell.transform.position.x) * playerMovement.lookDir, 
                                        currentSpell.transform.position.y+spellSpawnPoint.localPosition.y);
        GameObject spell = GameObject.Instantiate(currentSpell, transform.position + spawnPoint, transform.rotation) as GameObject;
        spell?.GetComponent<ColorSpell>().Initi(color, colorInventory.GetColorBuff(), gameObject, playerMovement.lookDir);
        transform.position= new Vector3(transform.position.x, transform.position.y-0.001f,transform.position.z);
    }

    /// <summary>
    /// Removes the attack root. Called by animation event
    /// </summary>
    public void RemoveAttackRoot()
    {
        attacking = false;
        playerMovement.movementRoot.SetTotalRoot("attackRoot", false);
    }

    /// <summary>
    /// Removes the player being locked in the air when attacking
    /// </summary>
    public void RemovePlayerAirlock()
    {
        body.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
    }
}
