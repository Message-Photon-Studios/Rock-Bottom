using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColorLibrary : MonoBehaviour
{
    [SerializeField] GameColor[] colors;
    [SerializeField] GameColor[] primaryColors;
    [SerializeField] GameColor[] secondaryColors;
    [SerializeField] GameColor rainbow;

    public GameColor GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }

    public GameColor GetRandomPrimaryColor()
    {
        return primaryColors[Random.Range(0, primaryColors.Length)];
    }

    public GameColor GetRandomSecondaryColor()
    {
        return secondaryColors[Random.Range(0, secondaryColors.Length)];
    }

    public bool IsComplemtarty(GameColor color1, GameColor color2)
    {
        if (color1 == null || color2 == null) return false;
        if (primaryColors.Contains<GameColor>(color1) && secondaryColors.Contains<GameColor>(color2)) return ComplimentCheck(color1, color2);
        if (primaryColors.Contains<GameColor>(color2) && secondaryColors.Contains<GameColor>(color1)) return ComplimentCheck(color2, color1);
        return false;
    }

    public bool ComplimentCheck(GameColor primary, GameColor secundary)
    {
        if (primary.name.Equals("Red") && secundary.name.Equals("Green")) return true;
        if (primary.name.Equals("Yellow") && secundary.name.Equals("Purple")) return true;
        if (primary.name.Equals("Blue") && secundary.name.Equals("Orange")) return true;
        return false;
    }
}
