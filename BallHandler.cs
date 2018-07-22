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

    public void HandlePointerEvent(BaseEventData data)
    {
        PointerEventData p_data = (PointerEventData)data;
        if (p_data.button == PointerEventData.InputButton.Left)
        {
            Vector2 click_location = p_data.pressPosition;
            FireHook(click_location);
        }
        else if(p_data.button == PointerEventData.InputButton.Right)
        {
            BreakHook();
        }
    }

    public void FireHook(Vector2 click_location)
    {
        
        // if there is already a rope, then first destroy it
        if (current_hook)
        {
            BreakHook();
        }

        // get mouse location in world space and the direction towards that location
        Vector2 mouse_loc = Camera.main.ScreenToWorldPoint(click_location);
        Vector2 direction = mouse_loc - (Vector2)transform.position;

        // instantiate the new hook and set its travel direction
        current_hook = (GameObject)Instantiate(hook_prefab, transform.position, Quaternion.identity);
        current_hook.GetComponent<GrappleHook>().travel_direction = direction;
    }

    public void BreakHook()
    {
        Destroy(current_hook);
        current_hook = null;
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
