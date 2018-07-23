using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{

    public Color[] colors;

    public ColorPalette()
    {
        colors = new Color[5];
    }

    public static Dictionary<string, int> object_index = new Dictionary<string, int>()
    {
        {"Background", 0},
        {"Level", 1},
        {"Player", 2},
        {"Trail", 3},
        {"Hook", 2},
        {"Rope", 4}

    };

    public Color GetColour(string tag)
    {
        return colors[object_index[tag]];

    }
}
