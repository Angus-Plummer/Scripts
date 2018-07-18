using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCornerColliders : MonoBehaviour {

    public GameObject corner_prefab;
    private float collider_radius = 0.1f;
    public float collider_separation; // target separation between corner colliders

// Use this for initialization
void Start () {

        // calc angular separation of colliders
        float diameter = transform.localScale.x;
        float circumference = Mathf.PI * diameter;
        // calc the number of colliders needed to reach target separation then round up to whole number to ensure even spacing
        float number_of_colliders = Mathf.Ceil(circumference / collider_separation);
        float angular_separation = 2f * Mathf.PI / number_of_colliders;

        // calculate the collider radius corrected for the size of the parent
        float scaledRadius = collider_radius / Mathf.Max(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));

        // go around circle, incrementing the angle by the separation
        float current_angle = 0;
        while (current_angle < 2f * Mathf.PI)
        {
            // create a new corner and give it the scaled radius
            GameObject new_corner = Instantiate(corner_prefab, transform.position, Quaternion.identity, transform);
            new_corner.GetComponent<CircleCollider2D>().radius = scaledRadius;

            // distance from centre of main circle. set colliders to be offset a little from the circle surface to make rope look better when wrapping around
            float radial_separation_fraction = 0.5f + scaledRadius * 0.5f;

            // set position of corner as x = dist. * cos(angle), y = dist. * sin(angle)
            new_corner.transform.localPosition = new Vector3(radial_separation_fraction * Mathf.Cos(current_angle), radial_separation_fraction * Mathf.Sin(current_angle));
            
            // increment the angle by the separation for next corner
            current_angle += angular_separation;
        }
    }
}
