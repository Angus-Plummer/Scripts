using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speed; // speed of platform
    // platform will move between two targets
    public Vector2 position_1; // initial target position (in local coords)
    public Vector2 position_2; // other target position (in local coords)
    public Vector2 target; // current position moving towards

    // positions in world coords
    private Vector2 world_position_1;
    private Vector2 world_position_2;

	// Set initial target as position 1
	void Start () {
        world_position_1 = (Vector2)transform.position + position_1;
        world_position_2 = (Vector2)transform.position + position_2;
        target = world_position_1;
	}
	
	// Update is called once per frame
	void Update () {
        // every frame move towards the target location at speed: hook_speed
        transform.position = Vector2.MoveTowards(transform.position, target, speed*Time.deltaTime);
        // if it has reached its target then swap targets
        if ((Vector2)transform.position == target)
        {
            SwapTarget();
        }
    }

    // swaps the target between the two positions
    private void SwapTarget()
    {
        if (target == world_position_1)
        {
            target = world_position_2;
        }
        else
        {
            target = world_position_1;
        }

    }

}
