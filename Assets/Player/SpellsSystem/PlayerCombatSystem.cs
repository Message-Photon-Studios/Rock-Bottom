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
    //[SerializeField] float defaultAttackDamage;
    [SerializeField] float defaultAttackForce;
    public int rainbowComboDamage = 20;
    [SerializeField] Transform spellSpawnPoint; //The spawn point for the spells. This will be automatically fliped on the x-level
    [SerializeField] PlayerDefaultAttack defaultAttackHitbox; //The object that controlls the default attack hitbox
    [SerializeField] Vector2 defaultAttackOffset; //The offset that the default attack will be set to
    [SerializeField] InputActionReference defaultAttackAction, specialAttackAction, verticalLookDir;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] Animator animator;
    [SerializeField] PlayerSounds playerSounds;

    public int defaultAttackDamage = 0;
    private bool attacking;
    private Rigidbody2D body;

    private bool defaultAirHit = false;
    private bool spellAirHit = false;
    private bool attackDoubleJumped = false;
    Action<InputAction.CallbackContext> specialAttackHandler;
    Action<InputAction.CallbackContext> defaultAttackHandler;


    #region Setup
    private void OnEnable() {

        specialAttackHandler = (InputAction.CallbackContext ctx) => SpecialAttackAnimation();
        defaultAttackHandler = (InputAction.CallbackContext ctx) => DefaultAttackAnimation();
        
        body = GetComponent<Rigidbody2D>();
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
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

    #region Default Attack

    /// <summary>
    /// Makes checks for and plays animation for default attack.
    /// </summary>
    private void DefaultAttackAnimation ()
    {
        if(!playerMovement.IsGrounded() && defaultAirHit) return;
        if(attacking) return;
        
        if(playerMovement.IsGrappeling())
        {
            playerMovement.WallAttackLock();
        }

        if(!playerMovement.IsGrounded()) defaultAirHit = true;
        {
            if(!attackDoubleJumped)
            {
                attackDoubleJumped = true;
                playerMovement.ResetDoubleJump();
            }
        }
        attacking = true;
        playerMovement.inAttackAnimation = true;

        animator.SetTrigger("defaultAttack");
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        playerMovement.movementRoot.SetTotalRoot("attackRoot", true);
}

    /// <summary>
    /// Handles the players default attack
    /// </summary>
    private void DefaultAttack()
    {
        playerSounds.PlayDefaultAttack();
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
    private void EnemyHitDefault((List<GameObject> absorbList, List<GameObject> pushList) enemies)
    {
        foreach (GameObject enemyObj in enemies.absorbList)
        {
            EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
            (GameColor absorb, int ammount) = enemy.AbsorbColor();
            if(absorb && ammount > 0) enemy.enemySounds?.PlayOnHit();
            if(defaultAttackDamage > 0)
                enemy.DamageEnemy(defaultAttackDamage);
            colorInventory.AddColor(absorb, ammount);
        }
        foreach (GameObject enemyObj in enemies.pushList)
        {
            if(enemyObj == null) continue;
            EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
            if (!enemy.IsKnockbackImune())
                enemy.GetComponent<Rigidbody2D>().AddForce(playerMovement.lookDir * Vector2.right * defaultAttackForce);
        }
    }

    #endregion

    #region Special Attack
    private GameObject currentSpell = null;
    /// <summary>
    /// Plays the animation for the special attack
    /// </summary>
    private void SpecialAttackAnimation()
    {
        if(!playerMovement.IsGrounded() && spellAirHit) return;
        currentSpell= colorInventory.GetActiveColorSpell().gameObject;
        if(currentSpell == null) return;
        if(attacking) return;
        if(!colorInventory.CheckActveColor()) return;
        if (!colorInventory.IsSpellReady()) return;

        if(playerMovement.IsGrappeling())
        {
            playerMovement.WallAttackLock();
        }
        
        if(!playerMovement.IsGrounded()) 
        {
            spellAirHit = true;
            if(!attackDoubleJumped) 
            {
                playerMovement.ResetDoubleJump();
                attackDoubleJumped = true;
            }
        }
        attacking = true;
        playerMovement.inAttackAnimation = true;
        string anim = currentSpell.GetComponent<ColorSpell>().GetAnimationTrigger();
        animator.SetTrigger(anim);
        playerMovement.movementRoot.SetTotalRoot("attackRoot", true);
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        playerSounds.PlayCastingSpell();
        colorInventory.DisableRotation();
    }

    /// <summary>
    /// Handles the players special attack. Called by animation event
    /// </summary>
    private void SpecialAttack()
    {
        GameColor color = colorInventory.UseActiveColor();
        colorInventory.EnableRotation();

        if(currentSpell == null || color == null) return;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x+currentSpell.transform.position.x) * playerMovement.lookDir, 
                                        currentSpell.transform.position.y+spellSpawnPoint.localPosition.y);
        GameObject spell = GameObject.Instantiate(currentSpell, transform.position + spawnPoint, transform.rotation) as GameObject;
        if(spell != null)
        {
            spell.GetComponent<ColorSpell>().Initi(color, colorInventory.GetColorBuff(), gameObject, playerMovement.lookDir);
            colorInventory.SetCoolDown(spell.GetComponent<ColorSpell>().coolDown); //When adding items to change the cooldown change it here! 
        }
            
        transform.position= new Vector3(transform.position.x, transform.position.y-0.001f,transform.position.z);
    }

    #endregion

    /// <summary>
    /// Removes the attack root. Called by animation event
    /// </summary>
    public void RemoveAttackRoot()
    {
        attacking = false;
        playerMovement.inAttackAnimation = false;
        playerMovement.movementRoot.SetTotalRoot("attackRoot", false);
    }

    /// <summary>
    /// Removes the player being locked in the air when attacking
    /// </summary>
    public void RemovePlayerAirlock()
    {
        body.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Reset the combat system to be grounded
    /// </summary>
    public void SetPlayerGrounded()
    {
        defaultAirHit = false;
        spellAirHit = false;
        attackDoubleJumped = false;
    }
}
