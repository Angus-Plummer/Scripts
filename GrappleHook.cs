using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Transform player;
    public Object node_prefab; 
    public GameObject rope; // first component of the rope

    public Vector2 travel_direction; // hook travel direction
    public float travel_speed; // travelling speed of hook
    private Vector2 hook_to_player;

    private Transform attached_obstacle; // obstacle to which the hook is attached
    private Vector2 local_offset; // position of hook relative to the attached obstacle

    private LineRenderer rope_line; // line renderer for the rope

    public LayerMask obstacle_layer; // layer mask for the colldiers which can be grappled on to
    
    private bool connected = false; // true if hook is connected to the player

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rope_line = GetComponent<LineRenderer>();
        node_prefab = Resources.Load("Prefabs/Rope Node");
        GetComponent<Rigidbody2D>().velocity = travel_direction.normalized * travel_speed;
    }

    // Update is called once per frame
    void Update()
    {
        hook_to_player = player.transform.position - transform.position;

        RaycastHit2D hit_info;
        // if the hook has not reached its target the move towards it
        if (!attached_obstacle)
        {
            // perform raycast to check if path between hook and player is obstructed
            hit_info = Physics2D.Raycast(transform.position, hook_to_player, hook_to_player.magnitude, obstacle_layer.value);
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
            local_offset = attached_obstacle.transform.InverseTransformPoint(transform.position);
            CreateRope();
            
        }
        // if the hook is connected then just update its position
        else
        {
            transform.position = attached_obstacle.transform.TransformPoint(local_offset);
        }
    }

    void LateUpdate()
    {
        RenderLine();
    }

    // function creates a rope node at the current location an sets it up
    private void CreateRope()
    {
        rope = (GameObject)Instantiate(node_prefab, transform.position, Quaternion.identity, transform);
        rope.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        rope.GetComponent<DistanceJoint2D>().distance = hook_to_player.magnitude;
        rope.GetComponent<RopeNode>().length = hook_to_player.magnitude;
        rope.GetComponent<RopeNode>().attached_object = transform;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        attached_obstacle = collider.transform;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // do raycast from hook in travel direction
        float raycast_distance = GetComponent<CircleCollider2D>().radius * 4;
        float desired_separation_from_collider = GetComponent<CircleCollider2D>().radius * 0.3f;
        RaycastHit2D hit_info = Physics2D.Raycast((Vector2)transform.position - travel_direction.normalized * raycast_distance, travel_direction.normalized, raycast_distance, obstacle_layer.value);
        if (hit_info.transform) // if hook's centre has passed through the collider
        {
            // move along the travel direction by an amount that the hook ends up the desired separation from the collider
            float angle = Vector2.Angle(-travel_direction, hit_info.normal);
            float correction_distance = raycast_distance - hit_info.distance + desired_separation_from_collider / Mathf.Cos(angle * Mathf.Deg2Rad);
            transform.position -= (Vector3)(travel_direction.normalized * correction_distance);
        }
        else // if the hook's centre has yet to pass through the collider
        {
            // do raycast from hook centre in travel direction
            hit_info = Physics2D.Raycast((Vector2)transform.position, travel_direction.normalized, raycast_distance, obstacle_layer.value);
            if (hit_info.transform)
            {
                // move along the travel direction by an amount that the hook ends up the desired separation from the collider
                float angle = Vector2.Angle(-travel_direction, hit_info.normal);
                float correction_distance = hit_info.distance - desired_separation_from_collider / Mathf.Cos(angle * Mathf.Deg2Rad);
                transform.position += (Vector3)(travel_direction.normalized * correction_distance);
            }
        }
    }

    // update the linerender
    void RenderLine()
    {
        rope_line.positionCount = 0;
        GameObject current_node = rope;
        // if there is no rope then first line point is hook
        if (!rope)
        {
            rope_line.positionCount++;
            rope_line.SetPosition(0, transform.position);
        }
        // if there is a rope then instead the line points are the rope node positions
        else
        {
            while (current_node)
            {
                rope_line.positionCount++;
                rope_line.SetPosition(rope_line.positionCount - 1, current_node.transform.position);
                current_node = current_node.GetComponent<RopeNode>().child_node;
            }
        }
        // add player to end
        rope_line.positionCount++;
        rope_line.SetPosition(rope_line.positionCount - 1, player.transform.position);
    }
}