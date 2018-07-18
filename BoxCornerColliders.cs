using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCornerColliders : MonoBehaviour
{

    public GameObject corner_prefab;
    private float collider_radius = 0.2f;
    private List<GameObject> colliders;
    // Use this for initialization
    void Start()
    {
        // inititalise the list
        colliders = new List<GameObject>();

        // calculate the radius corrected for the size of the parent
        float scaledRadius = collider_radius / Mathf.Max(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));

        // make 4 colliders. 1 at each corner
        for (int i = 0; i < 4; i++)
        {
            GameObject new_corner = Instantiate(corner_prefab, transform.position, Quaternion.identity, transform);
            new_corner.GetComponent<CircleCollider2D>().radius = scaledRadius;
            colliders.Add(new_corner);
        }
        // set the centre of each collider to one of the corners with a small offset away from the corner
        float offset_x = 0.5f + (collider_radius / transform.localScale.x) /4f;
        float offset_y = 0.5f + (collider_radius / transform.localScale.y) /4f;
        colliders[0].transform.localPosition = new Vector2(offset_x, offset_y);
        colliders[1].transform.localPosition = new Vector2(offset_x, -offset_y);
        colliders[2].transform.localPosition = new Vector2(-offset_x, offset_y);
        colliders[3].transform.localPosition = new Vector2(-offset_x, -offset_y);
    }
}
