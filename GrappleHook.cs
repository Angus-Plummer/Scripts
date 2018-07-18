using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Object hook_prefab;
    public Transform player;
    public GameObject child_hook;

    public Vector2 hook_target; // desired hook position
    public float hook_speed; // travelling speed of hook
    private Vector2 hook_to_player;

    private LineRenderer rope_line;

    public LayerMask obstacle_layer;
    public LayerMask corner_layer;
    private Collider2D last_corner = null;

    private float initial_rotation_direction = 0; // start as 0, when it connects then 1 = cw, -1 = ccw

    private bool connected = false; // true if hook is connected to next hook or the player
    public bool joining_child = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rope_line = GetComponent<LineRenderer>();
        hook_prefab = Resources.Load("Hook");
    }

    // Update is called once per frame
    void Update()
    {
        hook_to_player = player.transform.position - transform.position;
        // only update if the hook has no child
        if (child_hook == null)
        {
            RaycastHit2D hit_info;
            // if the hook has not reached its target
            if ((Vector2)transform.position != hook_target)
            {
                // every frame move towards the target location at speed: hook_speed
                transform.position = Vector2.MoveTowards(transform.position, hook_target, hook_speed);
                // perform raycast to check if path between player and hook is obstructed
                hit_info = Physics2D.Raycast(player.transform.position, -hook_to_player, hook_to_player.magnitude, obstacle_layer.value);
                // if there is a collider between them then destroy the hook
                if (hit_info.collider != null && hit_info.fraction < 0.95)
                {
                    Object.Destroy(this.gameObject);
                }
            }
            // if the hook is at target location and the player is not yet connected then connect the hook and the player
            else if (connected == false)
            {
                connected = true;
                // connect player with distance joint
                GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                GetComponent<DistanceJoint2D>().enabled = true;
                initial_rotation_direction = RotationDirection();
            }
            // if player is connected and hook is at target then check for rope collisions
            else
            {
                // check if there is a corner collider along the rope segment
                hit_info = Physics2D.Raycast(transform.position, hook_to_player, hook_to_player.magnitude, corner_layer.value);
                if (hit_info.collider != null)
                {
                    // to latch to corner we need that it is not the last corner unless its rotation direction has changed
                    if(hit_info.collider != last_corner || RotationDirection() == initial_rotation_direction)
                    {
                        SplitRope(hit_info.collider.transform.position);
                        last_corner = hit_info.collider;
                    }
                }
            }
            // if the hook isnt the first in the chain then we need to check if it needs to rejoin the parent
            if (transform.parent)
            {
                float current_rotation_direction = RotationDirection();
                Vector2 parent_hook_to_player = transform.parent.GetComponent<GrappleHook>().hook_to_player;
                hit_info = Physics2D.Raycast(transform.parent.position, parent_hook_to_player, parent_hook_to_player.magnitude, corner_layer.value);
                if (hit_info.collider == transform.parent.GetComponent<GrappleHook>().last_corner && current_rotation_direction != initial_rotation_direction)
                {
                    transform.parent.GetComponent<GrappleHook>().JoinWithChild();
                }
            }
        }
        // render the line
        RenderLine();
    }

    // split the rope at specified point
    void SplitRope(Vector2 split_location)
    {
        Vector2 hook_to_split = split_location - (Vector2)transform.position;
        // create child hook (dont need to attach to player as will do automatically as its connected flag will be false at start
        child_hook = (GameObject)Instantiate(hook_prefab, split_location, Quaternion.identity, transform);
        child_hook.GetComponent<GrappleHook>().hook_target = split_location;
        // child rope distance is current distance - current hook to corner
        child_hook.GetComponent<DistanceJoint2D>().autoConfigureDistance = false;
        child_hook.GetComponent<DistanceJoint2D>().distance = GetComponent<DistanceJoint2D>().distance - hook_to_split.magnitude;

        // disable rendering for the child hook
        child_hook.GetComponent<SpriteRenderer>().enabled = false;
        child_hook.GetComponent<LineRenderer>().enabled = false;
        
        // connect the joint to the child hook (both hooks are kinematic so wont actually do anything except configure the distance)
        GetComponent<DistanceJoint2D>().connectedBody = child_hook.GetComponent<Rigidbody2D>();
    }

    // joins the rope with the child
    public void JoinWithChild()
    {
        if (child_hook)
        {
            Destroy(child_hook);
            child_hook = null;
            GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }
    }

    // update the linerender
    void RenderLine()
    {
        // only first hook has line renderer (prevents part of line not being drawn when parts are destroyed)
        if (transform.parent == null)
        {
            int num_children = DeepChildCount(transform);
            rope_line.positionCount = num_children + 2; // main hook + children + player
            print("num_children = "+ num_children + " rope_line.positionCount = " + rope_line.positionCount);
            GameObject current_hook = transform.gameObject;

            // go through children and set line renderer points
            for (int i = 0; i < num_children + 1; i++)
            {
                rope_line.SetPosition(i, current_hook.transform.position);
                current_hook = current_hook.GetComponent<GrappleHook>().child_hook;

            }
            // add player
            rope_line.SetPosition(num_children + 1, player.transform.position);
        }
    }

    // helper function to count to total number of children (including their children)
    public static int DeepChildCount(Transform tf)
    {
        int count = 0;
        foreach(Transform child in tf.transform)
        {
            count += DeepChildCount(child);
            ++count;
        }
        return count;
    }

    // checks whether the player is rotating cw or ccw wrt. to the hook; 1 = cw, -1 = ccw
    private float RotationDirection()
    {
        Vector2 ang_vel = player.GetComponent<Rigidbody2D>().velocity / GetComponent<DistanceJoint2D>().distance;
        float sign = Mathf.Sign(ang_vel.x * hook_to_player.y - ang_vel.y * hook_to_player.x); // 1 = cw, -1 = ccw
        return sign;
    }
}