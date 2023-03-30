using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] Transform spellSpawnPoint;
    [SerializeField] InputActionReference specialAttackAction;
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
