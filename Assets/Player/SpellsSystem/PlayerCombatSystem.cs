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
    [SerializeField] InputActionReference defaultAttackAction, specialAttackAction; 
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ColorInventory colorInventory;
    [SerializeField] SpellInventory spellInventory;

    private void OnEnable() {
        specialAttackAction.action.performed += (_) =>{SpecialAttack();};
    }
    
    private void SpecialAttack()
    {

        GameObject obj = spellInventory.GetColorSpell();
        GameColor color = colorInventory.UseActiveColor();

        if(obj == null) return;

        Vector3 spawnPoint = new Vector3(spellSpawnPoint.localPosition.x * playerMovement.lookDir, spellSpawnPoint.localPosition.y);
        GameObject spell = GameObject.Instantiate(obj, transform.position + spawnPoint, transform.rotation) as GameObject;
        spell.GetComponent<ColorSpell>().Initi(color.colorEffect, colorInventory.GetColorBuff(), gameObject, playerMovement.lookDir);
    }
    
}
