using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

/// <summary>
/// This class handles the players attack actions and spawn the color spells
/// </summary>
public class PlayerCombatSystem : MonoBehaviour
{
    public int rainbowComboDamage = 20;
    [SerializeField] Transform spellSpawnPoint; //The spawn point for the spells. This will be automatically fliped on the x-level
    [SerializeField] InputActionReference attackAction1, attackAction2, attackAction3, attackAction4;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] Animator animator;
    [SerializeField] PlayerSounds playerSounds;
    [SerializeField] float bunnyCastTolerance;
    public int defaultAttackDamage = 0;
    private float bunnyCast = 0;

    /// <summary>
    /// Cascade damage will increase damage of spells each time a spell is cast, but will reset to zero when default attack is used.
    /// </summary>
    private int cascadeDamage = 0;
    public int maxCascadeDamage;
    private int bonusDamage;
    private int spellSorting = 0;
    private bool attacking;
    private Rigidbody2D body;
    private bool spellAirHit = false;
    private bool attackDoubleJumped = false;
    public UnityAction<string> onRecast;
    Action<InputAction.CallbackContext> attackHandler1, attackHandler2, attackHandler3, attackHandler4;
    public bool addColorMode {get; private set;} = false;
    public bool pickUpSpellMode = false;
    public ColorWell colorWell {get; private set;}


    #region Setup & Update
    private void OnEnable() {

        attackHandler1 = (InputAction.CallbackContext ctx) => AttackAnimation(0);
        attackHandler2 = (InputAction.CallbackContext ctx) => AttackAnimation(1);
        attackHandler3 = (InputAction.CallbackContext ctx) => AttackAnimation(2);        
        attackHandler4 = (InputAction.CallbackContext ctx) => AttackAnimation(4);
        
        body = GetComponent<Rigidbody2D>();
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;

        attackAction1.action.performed += attackHandler1;
        attackAction2.action.performed += attackHandler2;
        attackAction3.action.performed += attackHandler3;
        attackAction4.action.performed += attackHandler4;
    }

    private void Start()
    {
        GameManager.instance.onLevelLoaded += ResetSpellSortingCounter;
    }

    private void OnDisable()
    {
        attackAction1.action.performed -= attackHandler1;
        attackAction2.action.performed -= attackHandler2;
        attackAction3.action.performed -= attackHandler3;
        attackAction4.action.performed -= attackHandler4;

        GameManager.instance.onLevelLoaded -= ResetSpellSortingCounter;
    }

    private void ResetSpellSortingCounter()
    {
        spellSorting = 0;
    }

    void Update()
    {

        if (bunnyCast > 0 && bunnyCast >= Time.fixedTime)
        {
            AttackAnimation(activeSpellSlot);
        }
    }

    #endregion

    #region Attacks
    private GameObject currentSpell = null;
    private int activeSpellSlot = -1;   
    /// <summary>
    /// Plays the animation for the special attack
    /// </summary>
    public void AttackAnimation(int slotIndex)
    {
        if(slotIndex >= colorInventory.colorSlots.Count) return;
        
        if(pickUpSpellMode)
        {
            if(colorInventory.PickUp(slotIndex)) return;
        }

        if(addColorMode)
        {
            AddColorAnimation(colorInventory.GetSlot(slotIndex));
            return;
        }


        if (Time.timeScale == 0) return;
        if(!playerMovement.IsGrounded() && spellAirHit)
        {
            SetBunnySpell(slotIndex);
            return;
        }
        currentSpell= colorInventory.GetColorSpell(slotIndex).gameObject;
        if(currentSpell == null) return;
        if(attacking)
        {
            SetBunnySpell(slotIndex);
            return;
        }
        //if(!colorInventory.CheckActveColor()) return;
        if (!colorInventory.IsSpellReady(colorInventory.GetSlot(slotIndex))) return;



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
        activeSpellSlot = slotIndex;
        attacking = true;
        playerMovement.inAttackAnimation = true;
        string anim = currentSpell.GetComponent<ColorSpell>().GetAnimationTrigger();
        animator.SetTrigger(anim);
        playerMovement.movementRoot.SetTotalRoot("attackRoot", true);
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        playerSounds.PlayCastingSpell();
        colorInventory.DisableRotation();
        bunnyCast = -1;
    }

    /// <summary>
    /// Handles the players special attack. Called by animation event
    /// </summary>
    private void SpellAttack()
    {
        if(activeSpellSlot < 0) return;

        GameColor color = colorInventory.GetColorSlotColor(activeSpellSlot);
        if(currentSpell == null || color == null) return;
        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x+currentSpell.transform.position.x) * playerMovement.lookDir, 
                                        currentSpell.transform.position.y+spellSpawnPoint.localPosition.y);
        GameObject spell = GameObject.Instantiate(currentSpell, transform.position + spawnPoint, transform.rotation) as GameObject;
        if(spell != null)
        {
            ColorSpell spellStats = spell.GetComponent<ColorSpell>();
            spellStats.Initi(color, colorInventory.GetColorBuff(color), gameObject, playerMovement.lookDir, GetExtraDamage());
            colorInventory.UseColorSlot(colorInventory.colorSlots[activeSpellSlot]);
            spellStats.GetComponent<SpriteRenderer>().sortingOrder = spellSorting++;
            if (!spellStats.spawnKey.Equals(""))onRecast?.Invoke(spellStats.spawnKey);
            colorInventory.SetCoolDown(spell.GetComponent<ColorSpell>().coolDown, colorInventory.GetSlot(activeSpellSlot)); //When adding items to change the cooldown change it here! 
            colorInventory.SetRandomBuff();
            colorInventory.MixRandom();
        }
        colorInventory.EnableRotation();
        cascadeDamage++;
        if (cascadeDamage > maxCascadeDamage) cascadeDamage = maxCascadeDamage;
        transform.position= new Vector3(transform.position.x, transform.position.y-0.001f,transform.position.z);
    }

    public void PocketSpecialAttack(ColorSlot slot)
    {
        GameColor color = colorInventory.GetColorSlotColor(slot);
        ColorSpell spell = slot.colorSpell;
        if (spell == null || color == null) return;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x + spell.transform.position.x) * playerMovement.lookDir,
                                        spell.transform.position.y + spellSpawnPoint.localPosition.y);
        GameObject spellSpawn = GameObject.Instantiate(spell.gameObject, transform.position + spawnPoint, transform.rotation) as GameObject;
        if (spellSpawn != null)
        {

            spellSpawn.GetComponent<ColorSpell>().Initi(color, colorInventory.GetColorBuff(color), gameObject, playerMovement.lookDir, GetExtraDamage());
            colorInventory.UseColorSlot(slot);
            spellSpawn.GetComponent<SpriteRenderer>().sortingOrder = spellSorting++;
            colorInventory.SetRandomBuff();
            colorInventory.MixRandom(slot);
        }
        cascadeDamage++;
        if (cascadeDamage > maxCascadeDamage) cascadeDamage = maxCascadeDamage;

        transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
    }

    public void DashSpecialAttack(ColorSlot slot)
    {
        GameColor color = colorInventory.GetColorSlotColor(slot);
        ColorSpell spell = slot.colorSpell;
        if (spell == null || color == null) return;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x + spell.transform.position.x) * playerMovement.lookDir,
                                        spell.transform.position.y + spellSpawnPoint.localPosition.y);
        GameObject spellSpawn = GameObject.Instantiate(spell.gameObject, transform.position + spawnPoint, transform.rotation) as GameObject;
        if (spellSpawn != null)
        {
            spellSpawn.GetComponent<ColorSpell>().Initi(color, colorInventory.GetColorBuff(color), gameObject, playerMovement.lookDir, GetExtraDamage());
            colorInventory.UseColorSlot(slot);
            spellSpawn.GetComponent<SpriteRenderer>().sortingOrder = spellSorting++;
            colorInventory.SetCoolDown(spell.GetComponent<ColorSpell>().coolDown, slot);
            colorInventory.SetRandomBuff();
            colorInventory.MixRandom(slot);
        }
        cascadeDamage++;
        if (cascadeDamage > maxCascadeDamage) cascadeDamage = maxCascadeDamage;

        transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
    }

    public void DoubleJumpSpecialAttack(ColorSlot slot)
    {
        GameColor color = colorInventory.GetColorSlotColor(slot);
        ColorSpell spell = slot.colorSpell;
        if (spell == null || color == null) return;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x + spell.transform.position.x) * playerMovement.lookDir,
                                        spell.transform.position.y + spellSpawnPoint.localPosition.y);
        GameObject spellSpawn = GameObject.Instantiate(spell.gameObject, transform.position + spawnPoint, transform.rotation) as GameObject;
        if (spellSpawn != null)
        {
            int lookDir = playerMovement.lookDir;
            if(Time.time - playerMovement.lastFlipTime < 0.2f) lookDir *=-1;

            spellSpawn.GetComponent<ColorSpell>().Initi(color, colorInventory.GetColorBuff(color), gameObject, lookDir, GetExtraDamage());
            colorInventory.UseColorSlot(slot);
            spellSpawn.GetComponent<SpriteRenderer>().sortingOrder = spellSorting++;
            colorInventory.SetCoolDown(spell.GetComponent<ColorSpell>().coolDown, slot);
            colorInventory.SetRandomBuff();
            colorInventory.MixRandom(slot);
        }
        cascadeDamage++;
        if (cascadeDamage > maxCascadeDamage) cascadeDamage = maxCascadeDamage;

        transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
    }

    public int GetExtraDamage()
    {
        return cascadeDamage + colorInventory.GetColorMaxDamageBuff() + bonusDamage;
    }

    public void AddBonusDamage(int bonus)
    {
        bonusDamage += bonus;
    }

    #endregion

    #region Movement & root impacts
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
        spellAirHit = false;
        attackDoubleJumped = false;
    }

    private void SetBunnySpell(int spellSlot)
    {
        if (bunnyCast > Time.fixedTime) return;
        activeSpellSlot = spellSlot;
        bunnyCast = Time.fixedTime + bunnyCastTolerance;
    }

    #endregion

    #region Absorb Color

    public void EnableAbsorbColor(ColorWell colorWell)
    {
        this.colorWell = colorWell;

        if(colorWell.GetColorAmount() <= 0) 
        {
            DisableAbsorbColor();
            return;
        }
        addColorMode = true;
    }

    public void DisableAbsorbColor()
    {
        addColorMode = false;
    }

    public void MovedAwayFromWell(ColorWell movedAwayFrom)
    {
        if(colorWell == movedAwayFrom) colorWell = null;
    }

    private void AddColorAnimation (ColorSlot slot)
    {
        if(!addColorMode) return;
        if(colorWell == null) return;
        if(slot == null) return;
        if(colorWell.GetColorAmount() == 0 || colorWell.color == null) return;
        
        colorWell.UseWellAnimation(slot);
        
        animator.SetTrigger("gainColor");

        playerMovement.movementRoot.SetTotalRoot("colorWellActivation", true);
    }

    public void DeactivateAddColorMode()
    {
        addColorMode = false;
        playerMovement.movementRoot.SetTotalRoot("colorWellActivation", false);
    }
    #endregion

    #region Old Default Attack

    /*
    /// <summary>
    /// Makes checks for and plays animation for default attack.
    /// </summary>
    private void DefaultAttackAnimation ()
    {
        if (Time.timeScale == 0) return;
        if (!playerMovement.IsGrounded() && defaultAirHit) return;
        if(attacking) return;
        
        cascadeDamage = 0;

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
    */
    #endregion
}
