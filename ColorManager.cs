using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

    private ColorPalette color_palette;


    void Start()
    {
        color_palette = GameObject.FindGameObjectWithTag("Color Palette").GetComponent<ColorPalette>();
        UpdateColour();
    }

    private void UpdateColour()
    {
        Color sprite_colour = color_palette.GetColour(transform.tag);
        sprite_colour.a = 1f;
        GetComponent<SpriteRenderer>().color = sprite_colour;

        if(transform.tag == "Hook")
        {
            Color rope_colour = color_palette.GetColour("Rope");
            rope_colour.a = 1f;
            GetComponent<LineRenderer>().startColor = rope_colour;
            GetComponent<LineRenderer>().endColor = rope_colour;
        }
        else if (transform.tag == "Player")
        {
            Color trail_colour = color_palette.GetColour("Trail");
            trail_colour.a = 1f;
            GetComponent<TrailRenderer>().startColor = trail_colour;
            GetComponent<TrailRenderer>().endColor = trail_colour;
        }
    }
}
