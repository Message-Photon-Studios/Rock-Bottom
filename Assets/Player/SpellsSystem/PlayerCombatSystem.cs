using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using System.Linq;

/// <summary>
/// This class handles the players attack actions and spawn the color spells
/// </summary>
public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] Transform spellSpawnPoint; //The spawn point for the spells. This will be automatically fliped on the x-level
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] Animator animator;
    [SerializeField] PlayerSounds playerSounds;
    [SerializeField] float bunnyCastTolerance;
    public int greyExtraDamage = 0;
    private float bunnyCast = 0;

    /// <summary>
    /// Cascade damage will increase damage of spells each time a spell is cast, but will reset to zero when default attack is used.
    /// </summary>
    private int cascadeDamage = 0;
    public int maxCascadeDamage;
    private string lastSpellCast = "";
    private int bonusDamage;
    private int spellSorting = 0;
    private int emergecyBonusDamageMin = 0;
    private int emergecyBonusDamageMax = 0;
    private int d6 = 0;
    private bool attacking;
    private Rigidbody2D body;
    //private bool spellAirHit = false; // This bool checks so that only one spell can be cast in the air.
    private bool attackDoubleJumped = false;
    public UnityAction<string> onRecast;


    public bool addColorMode {get; private set;} = false;
    public ColorWell colorWell {get; private set;}
    public bool pickUpSpellMode {get; private set;} = false;
    public SpellPickup spellPickup {get; private set;}

    public Action<bool, ColorSpell> onSpellPickupMode;
    public Action<bool, GameColor> onColorPickupMode;

    #region Setup & Update
    private void OnEnable() {
        
        body = GetComponent<Rigidbody2D>();
        body.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    private void Start()
    {
        GameManager.instance.onLevelLoaded += ResetSpellSortingCounter;
        
        Player.instance.attackAction += AttackAnimation;
    }

    private void OnDisable()
    {
        Player.instance.attackAction -= AttackAnimation;

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
            spellPickup.PickedUp(slotIndex);
            SpellPickup(false, null);
            return;
        }

        if(addColorMode)
        {
            AddColorAnimation(slotIndex);
            return;
        }


        if (Time.timeScale == 0) return;
        /*if(!playerMovement.IsGrounded() && spellAirHit)
        {
            SetBunnySpell(slotIndex);
            return;
        }*/
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
            //spellAirHit = true;
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
        SpellAttack(colorInventory.GetSlot(activeSpellSlot), CastType.NORMAL);
    }

    public void SpellAttack(ColorSlot slot, CastType castType)
    {
        GameColor color = colorInventory.GetColorSlotColor(slot);
        ColorSpell spell = colorInventory.GetColorSpell(slot);
        if (spell == null || color == null) return;

        if (!spell.name.Equals(lastSpellCast)) cascadeDamage = 0;
        lastSpellCast = spell.name;

        Vector3 spawnPoint = new Vector3((spellSpawnPoint.localPosition.x + spell.gameObject.transform.position.x) * playerMovement.lookDir,
                                        spell.gameObject.transform.position.y + spellSpawnPoint.localPosition.y); //Creates spawn point for the spell

        GameObject spellSpawn = GameObject.Instantiate(spell.gameObject, transform.position + spawnPoint, transform.rotation) as GameObject; //Spawns the spell object

        if (spellSpawn != null)
        {
            int lookDir = playerMovement.lookDir;
            if (castType == CastType.JUMP) if (Time.time - playerMovement.lastFlipTime < 0.2f) lookDir *= -1;
            
            ColorSpell spellStats = spellSpawn.GetComponent<ColorSpell>();
            spellStats.Initi(color, colorInventory.GetColorBuff(color) + colorInventory.GetSlotBuff(slot), gameObject, lookDir, GetExtraDamage(color)); //Sets all the stats for the spell
            if (castType != CastType.EXTRA) colorInventory.UseColorSlot(slot); //Consumes the color after the spell has been spawned.
            spellStats.GetComponent<SpriteRenderer>().sortingOrder = spellSorting++; //Makes sure that the spells arent Z fighting. 
            if (!spellStats.spawnKey.Equals("")) onRecast?.Invoke(spellStats.spawnKey); //Triggers all spells that have some recast behaviour. EX Flail's chain breaks
            if (castType != CastType.HURT && castType != CastType.HIT && castType != CastType.EXTRA) colorInventory.SetCoolDown(spell.GetComponent<ColorSpell>().coolDown, slot); //Consumes ones spell Charge and sets it on cooldown.
            colorInventory.SetRandomBuff(); //Rerolls the D20 bonus
            colorInventory.MixRandom(slot); //Activates chaothic bottle

        }
        if (castType == CastType.NORMAL) colorInventory.EnableRotation();
        cascadeDamage++;
        if (cascadeDamage > maxCascadeDamage) cascadeDamage = maxCascadeDamage;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z); //It aint broke, dont touch it (We dont know what this does)

    }

    public IEnumerator ExtraSpell(ColorSlot slot, CastType castType)
    {
        yield return new WaitForSeconds(0.2f);
        SpellAttack(slot, castType);
    }

    public int GetExtraDamage(GameColor color)
    {
        int addDamage = 0;
        if (color == colorInventory.GetEmptyBottleColor()) addDamage += greyExtraDamage;
        return cascadeDamage + colorInventory.GetColorMaxDamageBuff() + bonusDamage + GetEmergencyDamage() + GetD6Damage(d6) + addDamage;
    }

    public void AddBonusDamage(int bonus)
    {
        bonusDamage += bonus;
    }

    public int GetEmergencyDamage()
    {
        int damage = 0;
        float relativeHealth = (float) GetComponent<PlayerStats>().GetHealth() / (float) GetComponent<PlayerStats>().GetMaxHealth();
        if (relativeHealth <= 0.5) damage += emergecyBonusDamageMin;
        if (relativeHealth <= 0.25) damage += emergecyBonusDamageMax;
        return damage;
    }

    public void AddEmergencyDamage(int min, int max)
    {
        emergecyBonusDamageMin += min;
        emergecyBonusDamageMax += max;
    }

    public int GetD6Damage(int amount)
    {
        int damage = 0;
        for (int i = 0; i < amount; i++)
        {
            damage += UnityEngine.Random.Range(1, 7);
        }
        return damage;
    }

    public void AddD6(int i)
    {
        d6 += i;
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
        //spellAirHit = false;
        attackDoubleJumped = false;
    }

    private void SetBunnySpell(int spellSlot)
    {
        if (bunnyCast > Time.fixedTime) return;
        activeSpellSlot = spellSlot;
        bunnyCast = Time.fixedTime + bunnyCastTolerance;
    }

    #endregion

    #region Absorb Color & Pick up spell

    public void SpellPickup (bool pickup, SpellPickup spellPickup)
    {
        pickUpSpellMode = pickup;
        if(pickup)
        {
            this.spellPickup = spellPickup;
            onSpellPickupMode?.Invoke(pickup, spellPickup.GetSpell());
        } else
        {
            this.spellPickup = null;
            onSpellPickupMode?.Invoke(pickup, null);
        }

        
    }

    public void EnableAbsorbColor(ColorWell colorWell)
    {
        Player.instance.playerMovement.movementRoot.SetTotalRoot("colorWellActivation", true);
        this.colorWell = colorWell;
        addColorMode = true;
        onColorPickupMode?.Invoke(true, colorWell.color);
    }

    public void MovedAwayFromWell(ColorWell movedAwayFrom)
    {
        if(colorWell == movedAwayFrom) colorWell = null;
    }

    private void AddColorAnimation (int slotIndex)
    {
        ColorSlot slot = colorInventory.GetSlot(slotIndex);
        if(!addColorMode) return;
        if(colorWell == null) return;
        if(slot == null) return;
        if(colorWell.GetColorAmount() == 0)
        {
            addColorMode = false;
            if(colorInventory.GetColorSlotColor(slotIndex).SharesRootColor(colorWell.color)) colorInventory.DivideColor(slotIndex);
            DeactivateAddColorMode();
            return;
        }
        
        colorWell.UseWellAnimation(slot);
        
        animator.SetTrigger("gainColor");
    }

    public void DeactivateAddColorMode()
    {
        addColorMode = false;
        playerMovement.movementRoot.SetTotalRoot("colorWellActivation", false);
        onColorPickupMode?.Invoke(false, null);
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

public enum CastType
{
    NORMAL, //When a spell is cast normally with a button press
    DASH, //The spell was cast when the player dashed
    JUMP, //The spell was cast when the player double jumped
    HURT, //The spell was cast when the player took damage
    HIT, //The spell was cast when another spell hit an enemy
    EXTRA //An additional spell that is casted for free (Both color and casting time/charge is free)
}
