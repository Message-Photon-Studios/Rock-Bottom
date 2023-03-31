using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// This class handles the players attack actions and spawn the color spells
/// </summary>
public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] Transform spellSpawnPoint; //The spawn point for the spells. This will be automatically fliped on the x-level
    [SerializeField] Transform defaultAttackHitbox; //The transform for the default attacks hitbox
    [SerializeField] Vector2 defaultAttackOffset; //The offset that the default attack will be set to
    [SerializeField] InputActionReference defaultAttackAction, specialAttackAction, verticalLookDir; 
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] SpellInventory spellInventory;
    [SerializeField] Animator animator;

    private bool attacking;

    #region Setup
    private void OnEnable() {
        specialAttackAction.action.performed += (_) =>{SpecialAttackAnimation();};
        defaultAttackAction.action.performed += (_) =>{DefaultAttack();};
    }

    private void OnDisable()
    {
        specialAttackAction.action.performed -= (_) =>{animator.SetTrigger("SpecialAttackHand");};
        defaultAttackAction.action.performed -= (_) =>{DefaultAttack();};
    }
    #endregion

    /// <summary>
    /// Handles the players default attack
    /// </summary>
    private void DefaultAttack()
    {
        Debug.Log("Default attack");
        float vertical = verticalLookDir.action.ReadValue<float>();
        float offsetX = (vertical == 0)? defaultAttackOffset.x * playerMovement.lookDir: 0;
        float offsetY = defaultAttackOffset.y * vertical;
        defaultAttackHitbox.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
        defaultAttackHitbox.gameObject.SetActive(true);
    }

    /// <summary>
    /// Plays the animation for the special attack
    /// </summary>
    private void SpecialAttackAnimation()
    {
        GameObject obj = spellInventory.GetColorSpell();
        if(obj == null) return;
        if(attacking) return;
        if(playerMovement.airTime > 0) return;
        attacking = true;
        string anim = obj.GetComponent<ColorSpell>().GetAnimationTrigger();
        animator.SetTrigger(anim);
        playerMovement.movementRoot.SetRoot("attackRoot", true);
    }

    /// <summary>
    /// Handles the players special attack. Called by animation event
    /// </summary>
    private void SpecialAttack()
    {

        GameObject obj = spellInventory.GetColorSpell();
        GameColor color = colorInventory.UseActiveColor();

        if(obj == null) return;

        Vector3 spawnPoint = new Vector3(spellSpawnPoint.localPosition.x * playerMovement.lookDir, spellSpawnPoint.localPosition.y);
        GameObject spell = GameObject.Instantiate(obj, transform.position + spawnPoint, transform.rotation) as GameObject;
        spell.GetComponent<ColorSpell>().Initi(color.colorEffect, colorInventory.GetColorBuff(), gameObject, playerMovement.lookDir);
    }

    /// <summary>
    /// Removes the attack root. Called by animation event
    /// </summary>
    private void RemoveAttackRoot()
    {
        attacking = false;
        defaultAttackHitbox.gameObject.SetActive(false);
        playerMovement.movementRoot.SetRoot("attackRoot", false);
    }
}
