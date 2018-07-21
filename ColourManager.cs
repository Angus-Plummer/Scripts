using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour {

    public GameObject colour_palettes;


    void Start()
    {
        colour_palettes = GameObject.FindGameObjectWithTag("Colour Palettes");
        UpdateColour();
    }

    private void UpdateColour()
    {
        print("key = " + transform.tag);
        Color sprite_colour = colour_palettes.GetComponent<ColourPalettes>().GetColour(transform.tag);
        sprite_colour.a = 1f;
        GetComponent<SpriteRenderer>().color = sprite_colour;

        if(transform.tag == "Hook")
        {
            Color rope_colour = colour_palettes.GetComponent<ColourPalettes>().GetColour("Rope");
            rope_colour.a = 1f;
            GetComponent<LineRenderer>().startColor = rope_colour;
            GetComponent<LineRenderer>().endColor = rope_colour;
        }
        else if (transform.tag == "Player")
        {
            Color trail_colour = colour_palettes.GetComponent<ColourPalettes>().GetColour("Trail");
            trail_colour.a = 1f;
            GetComponent<TrailRenderer>().startColor = trail_colour;
            GetComponent<TrailRenderer>().endColor = trail_colour;
        }
    }
}
