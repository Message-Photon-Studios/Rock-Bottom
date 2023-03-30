using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellInventory : MonoBehaviour
{
    [SerializeField] GameObject[] activeSpells;
    [SerializeField] InputActionReference[] spellActions;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ColorInventory colorInventory;

    private void OnEnable() {
        for (int i = 0; i < spellActions.Length; i++)
        {
            int index = i;
            spellActions[i].action.performed += (_) => {CastSpell(index);};
        }
    }

    private void CastSpell(int index)
    {
        if(index >= activeSpells.Length) return;
        GameObject spell = GameObject.Instantiate(activeSpells[index], transform.position, transform.rotation) as GameObject; //TODO fix rotation and position
        spell.GetComponent<ColorSpell>().Initi(colorInventory.GetActiveColorEffect(), colorInventory.GetColorBuff(), gameObject);
    }
    
}
