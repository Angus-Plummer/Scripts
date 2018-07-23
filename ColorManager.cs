using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour {

    private ColorPalette color_palette;
    [Range(0,4)]
    public int color_index;


    private void Start()
    {
        if (color_palette != GameManager.GetColorPalette())
        {
            color_palette = GameManager.GetColorPalette();
            UpdateColour();
        }
    }

    private void Update()
    {
        if (color_palette != GameManager.GetColorPalette())
        {
            color_palette = GameManager.GetColorPalette();
            UpdateColour();
        }
    }

    private void UpdateColour()
    {
        if (gameObject.layer == 5) // 5 = UI layer
        {
            if (transform.tag == "Text")
            {
                if (GetComponent<TMPro.TextMeshProUGUI>().enableVertexGradient)
                {
                    GetComponent<TMPro.TextMeshProUGUI>().colorGradientPreset = color_palette.GetTextGradient();
                    GetComponent<TMPro.TextMeshProUGUI>().outlineColor = color_palette.GetColor(0);
                }
                else
                {
                    GetComponent<TMPro.TextMeshProUGUI>().color = color_palette.GetColor(color_index);
                }
                
            }
            else
            {

                GetComponent<Image>().color = color_palette.GetColor(color_index);
            }
        }
        // if not on ui layer
        else
        {
            GetComponent<SpriteRenderer>().color = color_palette.GetColor(color_index);

            if (transform.tag == "Hook")
            {
                Color rope_colour = color_palette.GetColor(3);
                GetComponent<LineRenderer>().startColor = rope_colour;
                GetComponent<LineRenderer>().endColor = rope_colour;
            }
            else if (transform.tag == "Player")
            {
                Color trail_colour = color_palette.GetColor(4);
                GetComponent<TrailRenderer>().startColor = trail_colour;
                GetComponent<TrailRenderer>().endColor = trail_colour;
            }
        }
    }
}
