using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color Palette", menuName = "Color Palette")]
public class ColorPalette : ScriptableObject
{
    public new string name;
    [SerializeField]
    private Color[] colors;
    [SerializeField]
    private TMPro.TMP_ColorGradient text_gradient; 

    ColorPalette()
    {
        colors = new Color[5];
        for (int i = 0; i < colors.Length; i++){
            colors[i].a = 1;
        }
    }

    public Color GetColor(int index)
    {
        return colors[index];
    }

    public TMPro.TMP_ColorGradient GetTextGradient()
    {
        return text_gradient;
    }
}
