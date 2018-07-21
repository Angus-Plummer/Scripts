using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderForCircle : MonoBehaviour {

    public float collider_separation; // target linear separation between vertices

    // Use this for initialization
    void Start () {
        // calc angular separation of vertices
        float radius = 0.5f;
        float circumference = Mathf.PI * radius * 2f;
        // calc the number of vertices needed to reach target separation then round up to whole number to ensure even spacing
        int number_of_vertices = Mathf.CeilToInt(circumference / (collider_separation / transform.localScale.x));
        float angular_separation = 2f * Mathf.PI / (float)number_of_vertices;

        Vector2[] vertices = new Vector2[number_of_vertices];

        // go around circle, incrementing the angle by the separation
        float current_angle = 0;
        for(int i = 0; i < number_of_vertices; i++)
        {
            // set position of vertex as x = radius * cos(angle), y = radius * sin(angle)
            vertices[i] = new Vector2(radius * Mathf.Cos(current_angle), radius * Mathf.Sin(current_angle));

            // increment the angle by the separation for next corner
            current_angle += angular_separation;
        }
        // update the polygon 2D collider
        GetComponent<PolygonCollider2D>().points = vertices;
    }
}
