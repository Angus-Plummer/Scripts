using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    // prototype hook to fire from player
    public GameObject hook_prefab;

    // hook currently in use
    public GameObject current_hook;

    // layer mask for layers that the hook can latch on to
    public LayerMask hook_target_layers;


    // Update is called once per frame
    void Update()
    {
        // on left mouse click down
        if (Input.GetMouseButtonDown(0))
        {
            // if there is already a rope, then first destroy it
            if (current_hook)
            {
                Destroy(current_hook);
            }
            // get mouse location in world space and the direction towards that location
            Vector2 mouse_loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouse_loc - (Vector2)transform.position;

            // perform a raycast in the direction of the click
            RaycastHit2D hit_info = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, hook_target_layers.value);

            // if there was a collider hit then make the hook and set its target
            if (hit_info.collider)
            {
                // actual target location is where this raycast first meets a collider
                Vector2 actual_target = hit_info.point;
                // correct to have the hook a small normal displacement from the collider
                actual_target += hit_info.normal * hook_prefab.GetComponent<CircleCollider2D>().radius * 0.2f;

                // instantiate the new hook and set its target to be that calculated from the raycast
                current_hook = (GameObject)Instantiate(hook_prefab, transform.position, Quaternion.identity);
                current_hook.GetComponent<StiffRopeControl>().hook_target = actual_target;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // if there is already a rope, then first destroy it
            if (current_hook)
            {
                Destroy(current_hook);
            }
        }

        if(GetComponent<Rigidbody2D>().velocity == Vector2.zero && current_hook == null)
        {
            Respawn();
        }
    }

    // called when the player hits a wall
    void OnCollisionEnter2D(Collision2D col)
    {
        // if there is a hook active, then destroy it
        if (current_hook)
        {
            //Destroy(current_hook);
        }
    }

    void Respawn()
    {
        transform.position = Vector3.zero;
    }
}
