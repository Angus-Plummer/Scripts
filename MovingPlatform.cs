using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speed; // speed of platform
    // platform will move between two targets
    public Vector2 position_1; // initial target position
    public Vector2 position_2; // other target position
    private Vector2 target; // current position moving towards

	// Set initial target as position 1
	void Start () {
        target = position_1;
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
        if (target == position_1)
        {
            target = position_2;
        }
        else
        {
            target = position_1;
        }

    }

}
