using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakChainOnRecast : MonoBehaviour
{

    [SerializeField] SpringJoint2D chain;
    string spellKey;

    private void OnDisable()
    {
        GetComponent<ColorSpell>().GetPlayerObj().GetComponent<PlayerCombatSystem>().onRecast -= BreakChain;
    }

    // Start is called before the first frame update
    void Start()
    {
        spellKey = GetComponent<ColorSpell>().spawnKey;
        GetComponent<ColorSpell>().GetPlayerObj().GetComponent<PlayerCombatSystem>().onRecast += BreakChain;
    }

    public void BreakChain(string key)
    {
        if (key.Equals(spellKey))
        {
            if (chain == null)
            {
                this.enabled = false;
                return;
            }
            chain.enabled = true;
            chain.breakForce = 0;
            this.enabled = false;
        }
    }

}
