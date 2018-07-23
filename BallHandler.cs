using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallHandler : MonoBehaviour
{
    // prototype hook to fire from player
    public GameObject hook_prefab;

    // hook currently in use
    public GameObject current_hook;

    // layer mask for layers that the hook can latch on to
    public LayerMask hook_target_layers;

    // location where the ball spawns at
    public Vector2 spawn_location;

    private Vector2 position_last_frame; // used to ensure the velocity of the ball is maintained when breaking the rope

    private void LateUpdate()
    {
        position_last_frame = transform.position;
    }

    public void HandlePointerEvent(BaseEventData data)
    {
        PointerEventData p_data = (PointerEventData)data;
        if (!current_hook)
        {
            // get mouse location in world space and the direction towards that location
            Vector2 click_location = p_data.pressPosition;
            click_location = Camera.main.ScreenToWorldPoint(click_location);
            FireHook(click_location);
        }
        else
        {
            BreakHook();
        }
    }

    public void FireHook(Vector2 target)
    {
        
        // if there is already a rope, then first destroy it
        if (current_hook)
        {
            BreakHook();
        }
        
        Vector2 direction = target - (Vector2)transform.position;
        // instantiate the new hook and set its travel direction
        current_hook = (GameObject)Instantiate(hook_prefab, transform.position, Quaternion.identity);
        current_hook.GetComponent<GrappleHook>().travel_direction = direction;
    }

    public void BreakHook()
    {
        Destroy(current_hook);
        current_hook = null;
        Vector2 velocity = ((Vector2)transform.position - position_last_frame) / Time.deltaTime;
        GetComponent<Rigidbody2D>().velocity = velocity;
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
}
