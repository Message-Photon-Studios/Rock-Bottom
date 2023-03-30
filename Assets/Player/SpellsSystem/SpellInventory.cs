using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellInventory : MonoBehaviour
{
    [SerializeField] GameObject defaultColorSpell;
    [SerializeField] GameColor[] allColors;
    [SerializeField] ColorInventory colorInventory;
    Dictionary<GameColor, GameObject> colorSpellDict = new Dictionary<GameColor, GameObject>();

    private void Start() 
    {
        for (int i = 0; i < allColors.Length; i++)
        {
            colorSpellDict.Add(allColors[i], defaultColorSpell);
        }
    }

    /// <summary>
    /// Returns the color spell and game color for the game color for the active color slot.
    /// </summary>
    /// <returns></returns>
    public GameObject GetColorSpell()
    {
        GameColor color = colorInventory.CheckActveColor();
        if(color == null) return null;
        GameObject obj = colorSpellDict[color];
        return obj;
    }

    /// <summary>
    /// Change the color spell for the specified game color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="newColorSpell"></param>
    public void ChangeColorSpell(GameColor color, GameObject newColorSpell)
    {
        if(newColorSpell == defaultColorSpell || !colorSpellDict.ContainsValue(newColorSpell))
        {
            colorSpellDict[color] = newColorSpell;
        }
    }
    
}
