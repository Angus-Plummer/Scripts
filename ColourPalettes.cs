using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPalettes : MonoBehaviour
{

    public Color[] retro = new Color[5];
    public Color[] superhot = new Color[5];
    public Color[] retro_blue = new Color[5];
    public Color[] smoky_evening = new Color[5];
    public Color[] soft_evening = new Color[5];
    public Color[] salmon_soft = new Color[5];
    public Color[] retro_seventies = new Color[5];
    public Color[] black_white = new Color[5];
    public Color[] bright = new Color[5];
    public Color[] earth = new Color[5];
    public Color[] grey = new Color[5];
    public Color[] chocolate = new Color[5];
    public Color[] grapefruit = new Color[5];


    public Color[] current_palette;

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
        return current_palette[object_index[tag]];

    }

    private void Awake()
    {
        current_palette = superhot;
    }


}
