using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Transform player;
    public Object node_prefab; // CHANGE TO ROPE NODE PREFAB
    public GameObject rope; // first component of the rope

    public Vector2 hook_target; // desired hook position
    public float hook_speed; // travelling speed of hook
    private Vector2 hook_to_player;

    // MUST SET UP HOOK TO HAVE POSITION RELATIVE TO ATTACHED OBSTACLE. THEN UPDATE POSITION RELATIVE TO THE OBSTACLE
    private GameObject attached_obstacle; // obstacle to which the hook is attached

    private LineRenderer rope_line; // line renderer for the rope

    public LayerMask obstacle_layer; // layer mask for the colldiers which can be grappled on to
    
    private bool connected = false; // true if hook is connected to the player

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rope_line = GetComponent<LineRenderer>();
        node_prefab = Resources.Load("RopeNode");
    }

    // Update is called once per frame
    void Update()
    {
        hook_to_player = player.transform.position - transform.position;

        RaycastHit2D hit_info;
        // if the hook has not reached its target the move towards it
        if ((Vector2)transform.position != hook_target)
        {
            // every frame move towards the target location at speed: hook_speed
            transform.position = Vector2.MoveTowards(transform.position, hook_target, hook_speed);
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
            rope = (GameObject)Instantiate(node_prefab, transform.position, Quaternion.identity, transform);
            rope.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            rope.GetComponent<DistanceJoint2D>().distance = hook_to_player.magnitude;
            rope.GetComponent<RopeNode>().attached_object = transform;
        }
        RenderLine();
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